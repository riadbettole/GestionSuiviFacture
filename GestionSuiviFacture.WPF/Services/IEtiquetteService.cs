using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services
{
    public interface IEtiquetteService
    {
        Task<Etiquette> GetEtiquette(String numSequence);
    }
}
