using System.Diagnostics;
using System.Text.Json;
using System.Web;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services;

class CommandeService
{
    public Commande? _commande;
    public IEnumerable<BonDeLivraisonDto>? _bonDeLivraison;

    public Task<Commande?> GetCommande()
    {
        return Task.FromResult(_commande);
    }

    public async Task<Commande?> GetCommandeByFilterAsync(
        string? numSite = null,
        string? numCommande = null
    )
    {
        var queryString = BuildFilterQueryString(numSite, numCommande);

        var commandeDto = await FetchCommandDTOs(queryString);

        if (commandeDto != null)
        {
            _commande = MapToModel(commandeDto);
            _bonDeLivraison = MapToBonDeLivraison(commandeDto);
        }
        else
        {
            _commande = null;
        }

        return _commande;
    }

    private static async Task<CommandeDto?> FetchCommandDTOs(string queryString)
    {
        try
        {
            var response = await AuthenticatedHttpClient.GetAsync("Commande" + queryString);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            var commandeDto = JsonSerializer.Deserialize<CommandeDto>(jsonString, JsonConfig.DefaultOptions);

            return commandeDto;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching commandes: {ex.Message}");
            return null;
        }
    }

    private static string BuildFilterQueryString(string? numSite = null, string? numCommande = null)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrEmpty(numSite))
            parameters["n_site"] = numSite;
        if (!string.IsNullOrEmpty(numCommande))
            parameters["n_commande"] = numCommande;

        return parameters.Count > 0 ? "?" + parameters.ToString() : "";
    }

    private static Commande MapToModel(CommandeDto dto)
    {
        return new Commande(
            NomFournisseur: dto.LibelleFournisseur,
            CNUF: dto.Cnuf,
            Site: dto.LibelleSite,
            Rayon: dto.Rayon,
            Groupe: dto.Groupe,
            MontantTTC: dto.MontantBRV,
            DateCommande: dto.DateCommande,
            DateEcheance: dto.DateEcheance,
            BonDeLivraison: MapToModelBL(
                dto.BonsLivraison?.Values ?? Enumerable.Empty<BonDeLivraisonDto>()
            )
        );
    }

    private static List<BonDeLivraison> MapToModelBL(IEnumerable<BonDeLivraisonDto> dtos)
    {
        List<BonDeLivraison> bls = [];

        foreach (BonDeLivraisonDto dto in dtos)
        {
            bls.Add(new BonDeLivraison(dto.NumeroLivraison, dto.DateReception, dto.MontantTTC));
        }

        return bls;
    }

    private static IEnumerable<BonDeLivraisonDto> MapToBonDeLivraison(CommandeDto dto)
    {
        if (dto.BonsLivraison?.Values == null)
            return Enumerable.Empty<BonDeLivraisonDto>();

        return dto
            .BonsLivraison.Values.Select(blDto => new BonDeLivraisonDto
            {
                NumeroLivraison = blDto.NumeroLivraison,
                DateReception = blDto.DateReception,
                MontantTTC = blDto.MontantTTC,
            })
            .ToList();
    }
}
