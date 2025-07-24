using GestionSuiviFacture.WPF.Configuration;
using GestionSuiviFacture.WPF.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

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
    private StorageCredential _storageCredential;
    private readonly HttpClient _httpClient;
    private const string API_BASE_URL = "https://localhost:7167/api/v1";

    public event EventHandler? LogoutRequired;

    public int UserID => _userId;
    public string Username => _username;
    public bool IsAuthenticated =>
        !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;

    public AuthService(HttpClient httpClient, StorageCredential storageCredential)
    {
        _httpClient = httpClient;
        _storageCredential = storageCredential;
    }

    public async Task<string?> GetValidTokenAsync()
    {
        if (DateTime.UtcNow.AddMinutes(5) >= _tokenExpiry && !string.IsNullOrEmpty(_refreshToken))
        {
            await RefreshTokenAsync();
        }

        return IsAuthenticated ? _accessToken : null;
    }

    public async Task<bool> TryAutoLoginAsync()
    {
        if (!_storageCredential.HasStoredCredentials())
            return false;

        var (username, password) = _storageCredential.LoadCredentials();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;

        return await LoginAsync(username, password, false); // Don't save again
    }

    public async Task<bool> LoginAsync(string username, string password, bool rememberMe)
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

                if (rememberMe)
                {
                    _storageCredential.SaveCredentials(username, password);
                }

                return true;
            }
        }
        else if (response.StatusCode == HttpStatusCode.Locked)
        {
            throw new InvalidOperationException("Votre compte a été archivé. Contactez le support.");
        }
        return false;
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
            LogoutRequired?.Invoke(this, EventArgs.Empty);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh error: {ex.Message}");
            Logout();
            LogoutRequired?.Invoke(this, EventArgs.Empty);
            return false;
        }
        finally
        {
            // do repeated
        }
    }

    public void Logout()
    {
        _accessToken = "";
        _refreshToken = "";
        _tokenExpiry = DateTime.MinValue;
        _userId = 0;
        _username = "";
        _storageCredential.ClearCredentials();
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
