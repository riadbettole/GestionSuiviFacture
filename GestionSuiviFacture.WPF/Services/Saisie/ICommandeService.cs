using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services.Saisie;

public interface ICommandeService
{
    Task<Commande?> GetCommandeByFilterAsync(string? numSite = null, string? numCommande = null);
    Task<CommandeDto?> FetchCommandDTOs(string queryString);
}
