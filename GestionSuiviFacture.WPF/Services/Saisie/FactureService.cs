using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GestionSuiviFacture.WPF.Services.Saisie
{
    public class FactureService
    {
        private readonly HttpClient _httpClient;


        private const string _baseUrl = "https://localhost:7167/api/";

        public FactureService()
        {
            _httpClient = new HttpClient();
            RefreshAuthorizationHeader();
        }

        public async Task PostFacture()
        {

        }

        public async Task PostJsonAsync<T>(string relativeUrl, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/{relativeUrl.TrimStart('/')}", content);
            response.EnsureSuccessStatusCode();
        }

        private void RefreshAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                AuthService.JwtToken
            );
        }
    }
}
