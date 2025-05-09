using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services
{
    public interface IEtiquetteService
    {
        Task<IEnumerable<Etiquette>> GetEtiquetteByFilter(
            string? numSequence = null,
            DateTime? debut = null,
            DateTime? fin = null,
            string? numCommande = null,
            string? cnuf = null,
            StatusEtiquette? statusEtiquette = null
        );
    }
}
