using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using Newtonsoft.Json.Linq;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

public class EtiquetteService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://localhost:7167/api/etiquette";

    public EtiquetteService()
    {
        _httpClient = new HttpClient();
        RefreshAuthorizationHeader();
    }

    public async Task<PaginatedResult<Etiquette>> GetEtiquettesByFilterAsync(
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        StatusEtiquette? statut = null,
        string? n_Sequence = null,
        string? n_Commande = null,
        string? cnuf = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var queryString = BuildFilterQueryString(
            dateDebut,
            dateFin,
            statut,
            n_Sequence,
            n_Commande,
            cnuf,
            pageNumber,
            pageSize);

        RefreshAuthorizationHeader();

        var response = await FetchPaginatedEtiquetteDTOs($"{BaseUrl}/filter{queryString}");

        return new PaginatedResult<Etiquette>
        {
            Items = response.Items.Select(MapToModel),
            TotalCount = response.TotalCount
        };
    }

    private string BuildFilterQueryString(
        DateTime? dateDebut,
        DateTime? dateFin,
        StatusEtiquette? statut,
        string? n_Sequence,
        string? n_Commande,
        string? cnuf,
        int pageNumber,
        int pageSize)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);

        if (dateDebut.HasValue)
            parameters["dateDebut"] = dateDebut.Value.ToString("yyyy-MM-dd");

        if (dateFin.HasValue)
            parameters["dateFin"] = dateFin.Value.ToString("yyyy-MM-dd");

        if (statut.HasValue)
            parameters["statut"] = ReverseMapEnumStatus(statut.Value);

        if (!string.IsNullOrEmpty(n_Sequence))
            parameters["n_Sequence"] = n_Sequence;

        if (!string.IsNullOrEmpty(n_Commande))
            parameters["n_Commande"] = n_Commande;

        if (!string.IsNullOrEmpty(cnuf))
            parameters["cnuf"] = cnuf;

        parameters["PageNumber"] = pageNumber.ToString();
        parameters["PageSize"] = pageSize.ToString();

        return "?" + parameters.ToString();
    }

    private void RefreshAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            AuthService.JwtToken
        );
    }

    private async Task<PaginatedResult<EtiquetteDTO>> FetchPaginatedEtiquetteDTOs(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(json);

            var items = jsonObject["items"] ?? jsonObject["Items"];
            var totalCount = jsonObject["totalCount"]?.Value<int>() ?? 0;

            return new PaginatedResult<EtiquetteDTO>
            {
                Items = items?.ToObject<List<EtiquetteDTO>>() ?? new List<EtiquetteDTO>(),
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching etiquettes: {ex.Message}");
            throw;
        }
    }

    private Etiquette MapToModel(EtiquetteDTO dto)
    {
        return new Etiquette(
            Magasin: dto.site?.n_Site,
            Cnuf: dto.fournisseur?.cnuf,
            NumSequence: dto.n_sequence,
            NumFacture: dto.facture?.n_Facture,
            NumReception: dto.reception?.n_Reception.ToString(),
            NumCommande: dto.commande?.n_Commande,
            DateTraitement: dto.date_Traitement,
            DateReception: dto.reception?.date_Reception ?? DateTime.MinValue,
            DateCommande: dto.commande?.date_Commande ?? DateTime.MinValue,
            DateRapprochement: DateTime.MinValue,
            Status: MapEnumStatus(dto.statut),
            Fournisseur: dto.fournisseur?.libelle_Fournisseur,
            MontantBF: 0,
            MontantFacture: dto.facture?.total_TTC ?? 0,
            Utilisateur: null,
            UtilisateurAnnule: null,
            MotifAnnulation: null,
            Description: null,
            LibelleSite: dto.site?.libelle_Site,
            GroupeSite: dto.commande?.groupe
        );
    }

    private StatusEtiquette MapEnumStatus(int status) =>
        status switch
        {
            0 => StatusEtiquette.OK,
            1 => StatusEtiquette.NOK,
            2 => StatusEtiquette.ANNULE,
            _ => throw new ArgumentOutOfRangeException(nameof(status), "Unknown status value"),
        };

    private string ReverseMapEnumStatus(StatusEtiquette status) =>
        status switch
        {
            StatusEtiquette.OK => "OK",
            StatusEtiquette.NOK => "NOK",
            StatusEtiquette.ANNULE => "Annulee",
            _ => throw new ArgumentOutOfRangeException(nameof(status), "Unknown status value"),
        };
}
