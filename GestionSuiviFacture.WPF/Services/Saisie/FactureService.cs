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
        public async Task<bool> PostEtiquetteAsync(EtiquetteDTO etiquetteDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(etiquetteDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await AuthenticatedHttpClient.PostAsync("Etiquette", httpContent);
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
    }
}
