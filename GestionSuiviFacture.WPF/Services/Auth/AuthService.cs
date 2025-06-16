using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using GestionSuiviFacture.WPF.Configuration;
using GestionSuiviFacture.WPF.Services.Utilities;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Services;

class TokenResponse
{
    public string? AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
}

public class AuthService : IAuthService
{
    private string _accessToken = "";
    private string _refreshToken = "";
    private DateTime _tokenExpiry = DateTime.MinValue;
    private int _userId = 0;
    private string _username = "";

    private readonly HttpClient _httpClient;
    private readonly WindowManager _windowManager;
    private const string API_BASE_URL = "https://localhost:7167/api/v1";

    public event EventHandler<bool>? AuthenticationChanged;

    public int UserID => _userId;
    public string Username => _username;
    public bool IsAuthenticated =>
        !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;

    public AuthService(HttpClient httpClient, WindowManager windowManager)
    {
        _httpClient = httpClient;
        _windowManager = windowManager;
    }

    public async Task<string?> GetValidTokenAsync()
    {
        if (DateTime.UtcNow.AddMinutes(5) >= _tokenExpiry && !string.IsNullOrEmpty(_refreshToken))
        {
            await RefreshTokenAsync();
        }

        return IsAuthenticated ? _accessToken : null;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var payload = new { Username = username, Password = password };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{API_BASE_URL}/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
                    result,
                    JsonConfig.DefaultOptions
                );
                if (tokenResponse != null)
                {
                    SetTokens(tokenResponse.AccessToken, tokenResponse.RefreshToken);
                    ExtractUserInfo();
                    AuthenticationChanged?.Invoke(this, true);
                    return true;
                }
            }
            else if (response.StatusCode == (HttpStatusCode)423) // Locked
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }

            return false;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw to preserve the specific error message
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var payload = new { RefreshToken = _refreshToken };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{API_BASE_URL}/refresh",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
                    result,
                    JsonConfig.DefaultOptions
                );

                if (tokenResponse != null)
                {
                    SetTokens(tokenResponse.AccessToken, tokenResponse.RefreshToken);
                    return true;
                }
            }

            // If refresh fails, clear all tokens
            Logout();
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh error: {ex.Message}");
            Logout();
            return false;
        }
    }

    public void Logout()
    {
        _accessToken = "";
        _refreshToken = "";
        _tokenExpiry = DateTime.MinValue;
        _userId = 0;
        _username = "";

        AuthenticationChanged?.Invoke(this, false);

        _windowManager.OnLogout();
    }

    private void SetTokens(string? accessToken, string? refreshToken)
    {
        _accessToken = accessToken ?? "";
        _refreshToken = refreshToken ?? "";

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(_accessToken);
        _tokenExpiry = jwtToken.ValidTo;
    }

    private void ExtractUserInfo()
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(_accessToken);
        var claims = jwtToken.Claims.ToList();

        _userId = Convert.ToInt32(claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "0");
        _username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
    }
}
