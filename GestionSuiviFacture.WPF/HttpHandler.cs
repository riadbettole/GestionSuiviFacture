// Add this new static class for easy HTTP calls with automatic auth
using System.Net.Http;

namespace GestionSuiviFacture.WPF.Services
{
    public static class AuthenticatedHttpClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://localhost:7167/api/v1";

        // Static method for GET requests
        public static async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{BaseUrl}/{endpoint}");

            // Handle 401 - try refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await SetAuthHeaderAsync(forceRefresh: true);
                response = await _httpClient.GetAsync($"{BaseUrl}/{endpoint}");
            }

            return response;
        }

        // Static method for POST requests
        public static async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PostAsync($"{BaseUrl}/{endpoint}", content);

            // Handle 401 - try refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await SetAuthHeaderAsync(forceRefresh: true);
                response = await _httpClient.PostAsync($"{BaseUrl}/{endpoint}", content);
            }

            return response;
        }

        // Static method for PUT requests
        public static async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PutAsync($"{BaseUrl}/{endpoint}", content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await SetAuthHeaderAsync(forceRefresh: true);
                response = await _httpClient.PutAsync($"{BaseUrl}/{endpoint}", content);
            }

            return response;
        }

        // Static method for DELETE requests
        public static async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await SetAuthHeaderAsync(forceRefresh: true);
                response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
            }

            return response;
        }

        private static async Task SetAuthHeaderAsync(bool forceRefresh = false)
        {
            string? token;

            if (forceRefresh)
            {
                // Force a token refresh
                token = await AuthService.GetValidTokenAsync();
            }
            else
            {
                // Use existing token or refresh if needed
                token = await AuthService.GetValidTokenAsync();
            }

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}