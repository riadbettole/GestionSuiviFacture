using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Services.Saisie;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using GestionSuiviFacture.WPF.Configuration;
using GestionSuiviFacture.WPF.Services;

public class FactureService : IFactureService
{
    private readonly IAuthenticatedHttpClient _httpClient;

    public FactureService(IAuthenticatedHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> PostEtiquetteAsync(EtiquetteDto etiquetteDto)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(
                etiquetteDto,
                JsonConfig.DefaultOptions
            );

            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Etiquette", httpContent);
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error posting etiquette: {ex.Message}");
            return false;
        }
    }
}