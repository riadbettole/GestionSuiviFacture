using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services;

public interface IEtiquetteService
{
    Task<PaginatedResult<Etiquette>> GetEtiquettesByFilterAsync(EtiquetteFilterRequest request);
    Task<PaginatedResult<EtiquetteDto>> FetchPaginatedEtiquetteDTOs(string queryString);
}
