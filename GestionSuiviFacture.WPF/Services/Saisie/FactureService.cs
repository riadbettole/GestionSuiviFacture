using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Services.Saisie;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using GestionSuiviFacture.WPF.Configuration;
using GestionSuiviFacture.WPF.Services.Network;

public class FactureService : IFactureService
{
    private readonly IAuthenticatedHttpClient _httpClient;

    public FactureService(IAuthenticatedHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(string NSequence, DateTime DateTraitement)> PostEtiquetteAsync(EtiquetteDto etiquetteDto)
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

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<EtiquetteApiResponse>(responseContent, JsonConfig.DefaultOptions);

            return (responseData.NSequence, responseData.DateTraitement);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error posting etiquette: {ex.Message}");
            return ("Erreur", new DateTime());
        }
    }
}

public class EtiquetteApiResponse
{
    public string Message { get; set; }
    public string NSequence { get; set; }
    public DateTime DateTraitement { get; set; }
}