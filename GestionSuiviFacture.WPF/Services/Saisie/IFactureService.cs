using GestionSuiviFacture.WPF.DTOs;

namespace GestionSuiviFacture.WPF.Services.Saisie;

public interface IFactureService
{
    Task<bool> PostEtiquetteAsync(EtiquetteDto etiquetteDto);
}
