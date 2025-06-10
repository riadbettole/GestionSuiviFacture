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
    private const string BaseUrl = "https://localhost:7167/api/v1/etiquette";

    public EtiquetteService()
    {
        _httpClient = new HttpClient();
        RefreshAuthorizationHeader();
    }

    public async Task<PaginatedResult<Etiquette>> GetEtiquettesByFilterAsync(
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int? statut = null,
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
        int? statut,
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
            parameters["statut"] = Convert.ToString(statut);

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

            var last = items?.ToObject<List<EtiquetteDTO>>() ?? new List<EtiquetteDTO>();

            return new PaginatedResult<EtiquetteDTO>
            {
                Items = last,
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
        var lignesFacture = dto.LigneFactureDTOs?.Select(lf => new LigneFacture
        {
            Id = 0,  // fix that tmrw urgent
            Taux = lf.Taux,
            MontantHT = lf.MontantHT,
        }).ToList() ?? new List<LigneFacture>();

        return new Etiquette(
            NumSequence: dto.NSequence,
            Status: dto.Statut,
            DateTraitement: dto.DateTraitement,

            NumCommande: dto.NumCommande,
            Fournisseur: dto.LibelleFournisseur,
            MontantBRV: dto.MontantBRV ?? 0.0,
            Magasin: dto.NSite,
            DateCommande: dto.DateCommande,
            Cnuf: dto.Cnuf,
            LibelleSite: dto.Site,
            GroupeSite: dto.Groupe,

            NumFacture: dto.NumFacture,
            DateFacture: dto.DateFacture,
            MontantFacture: dto.MontantTTCFacture ?? 0.0,
            LignesFacture: lignesFacture,

            Utilisateur: dto.UtilisateurNom,

            UtilisateurAnnule: dto.UtilisateurNomAnnulation,
            DateAnnulation: dto.DateAnnulation,
            MotifAnnulation: MapIntMotif(dto.MotifAnnulation ?? 8),
            DescriptionAnnulation: dto.DescriptionAnnulation
        );
    }

    private string MapIntMotif(int motif) =>
       motif switch
       {
           0 => "Erreur de calcul",
           1 => "Bon de Commande Erroné",
           2 => "Relevé",
           3 => "Manque complement",
           4 => "Dossier non conforme",
           5 => "Manque HT",
           6 => "Erreur de prix",
           7 => "Deja traité",
           8 => "aucun",
           _ => throw new ArgumentOutOfRangeException(nameof(motif), "Unknown status value"),
       };
}
