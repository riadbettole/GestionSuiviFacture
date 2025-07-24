using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.Services.Network;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

public class EtiquetteService : IEtiquetteService
{
    private readonly IAuthenticatedHttpClient _httpClient;

    public EtiquetteService(IAuthenticatedHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaginatedResult<Etiquette>> GetEtiquettesByFilterAsync(EtiquetteFilterRequest request)
    {
        var queryString = EtiquetteUtils.BuildFilterQueryString(request);
        var response = await FetchPaginatedEtiquetteDTOs(queryString);

        return new PaginatedResult<Etiquette>
        {
            Items = response.Items.Select(EtiquetteUtils.MapToModel),
            TotalCount = response.TotalCount,
        };
    }

    public async Task<PaginatedResult<EtiquetteDto>> FetchPaginatedEtiquetteDTOs(string queryString)
    {
        try
        {
            var response = await _httpClient.GetAsync("Etiquette/filter" + queryString);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(json);

            var items = jsonObject["items"] ?? jsonObject["Items"];
            var totalCount = jsonObject["totalCount"]?.Value<int>() ?? 0;

            var last = items?.ToObject<List<EtiquetteDto>>() ?? new List<EtiquetteDto>();

            return new PaginatedResult<EtiquetteDto> { Items = last, TotalCount = totalCount };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching etiquettes: {ex.Message}");
            throw;
        }
    }
}