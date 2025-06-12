using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services;

public interface IEtiquetteService
{
    Task<IEnumerable<Etiquette>> GetEtiquettesByFilterAsync(
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int? statut = null,
        string? n_Sequence = null,
        string? n_Commande = null,
        string? cnuf = null
    );
}
