using System.Net.Http;

namespace GestionSuiviFacture.WPF.Services.Network;

public interface IAuthenticatedHttpClient
{
    Task<HttpResponseMessage> GetAsync(string endpoint);
    Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content);
    Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content);
    Task<HttpResponseMessage> DeleteAsync(string endpoint);
}

public class AuthenticatedHttpClient : IAuthenticatedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string BaseUrl = "https://localhost:7167/api/v1";

    public AuthenticatedHttpClient(IAuthService authService)
    {
        _httpClient = new HttpClient();
        _authService = authService;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
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

    public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
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

    public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
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

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
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

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetValidTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}