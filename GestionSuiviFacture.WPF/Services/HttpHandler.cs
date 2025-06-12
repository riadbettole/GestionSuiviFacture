// Add this new static class for easy HTTP calls with automatic auth
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;

namespace GestionSuiviFacture.WPF.Services;

public static class AuthenticatedHttpClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string BaseUrl = "https://localhost:7167/api/v1";

    public static async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.GetAsync($"{BaseUrl}/{endpoint}");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await SetAuthHeaderAsync();
            response = await _httpClient.GetAsync($"{BaseUrl}/{endpoint}");
        }

        return response;
    }

    public static async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsync($"{BaseUrl}/{endpoint}", content);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await SetAuthHeaderAsync();
            response = await _httpClient.PostAsync($"{BaseUrl}/{endpoint}", content);
        }

        return response;
    }

    public static async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PutAsync($"{BaseUrl}/{endpoint}", content);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await SetAuthHeaderAsync();
            response = await _httpClient.PutAsync($"{BaseUrl}/{endpoint}", content);
        }

        return response;
    }

    public static async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await SetAuthHeaderAsync();
            response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
        }

        return response;
    }

    private static async Task SetAuthHeaderAsync()
    {
        string? token;

        token = await AuthService.GetValidTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}