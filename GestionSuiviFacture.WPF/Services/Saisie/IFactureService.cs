using GestionSuiviFacture.WPF.DTOs;

namespace GestionSuiviFacture.WPF.Services.Saisie;

public interface IFactureService
{
    Task<(string NSequence, DateTime DateTraitement)> PostEtiquetteAsync(EtiquetteDto etiquetteDto);
}
