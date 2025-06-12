using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace GestionSuiviFacture.WPF.Services;

public class AuthService
{
    private static string _accessToken = "";
    private static string _refreshToken = "";
    private static DateTime _tokenExpiry = DateTime.MinValue;
    private static int _userId = 0;
    private static string _username = "";
    private static string _role = "";

    private static readonly HttpClient _httpClient = new HttpClient();
    private const string API_BASE_URL = "https://localhost:7167/api/v1";

    public static string JwtToken => _accessToken;
    public static int UserID => _userId;
    public static string Username => _username;
    public static string Role => _role;
    public static bool IsAuthenticated =>
        !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;

    public static async Task<string?> GetValidTokenAsync()
    {
        if (DateTime.UtcNow.AddMinutes(5) >= _tokenExpiry && !string.IsNullOrEmpty(_refreshToken))
        {
            await RefreshTokenAsync();
        }

        return IsAuthenticated ? _accessToken : null;
    }

    public static async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var payload = new { Username = username, Password = password };

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{API_BASE_URL}/auth/login",
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
                    ExtractUserInfo();
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }

    private static async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var payload = new { RefreshToken = _refreshToken };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{API_BASE_URL}/auth/refresh",
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

            // If refresh fails, clear all tokens SHOULD REALLY LOGOUT
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

    public static void Logout()
    {
        _accessToken = "";
        _refreshToken = "";
        _tokenExpiry = DateTime.MinValue;
        _userId = 0;
        _username = "";
        _role = "";
    }

    private static void SetTokens(string? accessToken, string? refreshToken)
    {
        _accessToken = accessToken ?? "";
        _refreshToken = refreshToken ?? "";

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(_accessToken);
        _tokenExpiry = jwtToken.ValidTo;
    }

    private static void ExtractUserInfo()
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(_accessToken);
        var claims = jwtToken.Claims.ToList();

        _userId = Convert.ToInt32(claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "0");
        _username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        _role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
    }

    private sealed class TokenResponse
    {
        public string? AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
