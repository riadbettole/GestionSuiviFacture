using System.Diagnostics;
using System.Text.Json;
using GestionSuiviFacture.WPF.Configuration;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services.Network;
using GestionSuiviFacture.WPF.Services.Saisie;

namespace GestionSuiviFacture.WPF.Services;

public class CommandeService : ICommandeService
{
    private readonly IAuthenticatedHttpClient _httpClient;
    private Commande? _commande;
    private IEnumerable<BonDeLivraisonDto>? _bonDeLivraison;

    public CommandeService(IAuthenticatedHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Commande?> GetCommande()
    {
        return Task.FromResult(_commande);
    }

    public async Task<Commande?> GetCommandeByFilterAsync(
        string? numSite = null,
        string? numCommande = null)
    {
        var queryString = CommandeUtils.BuildFilterQueryString(numSite, numCommande);
        var commandeDto = await FetchCommandDTOs(queryString);

        if (commandeDto != null)
        {
            _commande = CommandeUtils.MapToModel(commandeDto);
            _bonDeLivraison = CommandeUtils.MapToBonDeLivraison(commandeDto);
        }
        else
        {
            _commande = null;
        }

        return _commande;
    }

    public async Task<CommandeDto?> FetchCommandDTOs(string queryString)
    {
        try
        {
            var response = await _httpClient.GetAsync("Commande" + queryString);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var commandeDto = JsonSerializer.Deserialize<CommandeDto>(
                jsonString,
                JsonConfig.DefaultOptions
            );

            return commandeDto;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching commandes: {ex.Message}");
            return null;
        }
    }
}