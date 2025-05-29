using GestionSuiviFacture.WPF.DTOs;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GestionSuiviFacture.WPF.Services.Saisie
{
    class FactureService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7167/api";

        public FactureService()
        {
            _httpClient = new HttpClient();
            RefreshAuthorizationHeader();
        }

        public async Task<bool> PostEtiquetteAsync(EtiquetteFrontendDTO etiquetteDto)
        {
            try
            {
                RefreshAuthorizationHeader();

                var jsonContent = JsonSerializer.Serialize(etiquetteDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/Etiquette", httpContent);
                response.EnsureSuccessStatusCode();

                Debug.WriteLine("Etiquette created successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error posting etiquette: {ex.Message}");
                return false;
            }
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
