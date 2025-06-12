using System.Diagnostics;
using System.Web;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using Newtonsoft.Json.Linq;

namespace GestionSuiviFacture.WPF.Services;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

public class EtiquetteService
{
    public class EtiquetteFilterRequest
    {
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int? Statut { get; set; }
        public string? N_Sequence { get; set; }
        public string? N_Commande { get; set; }
        public string? Cnuf { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public async Task<PaginatedResult<Etiquette>> GetEtiquettesByFilterAsync(EtiquetteFilterRequest request)
    {
        var queryString = BuildFilterQueryString(request);

        var response = await FetchPaginatedEtiquetteDTOs(queryString);
        return new PaginatedResult<Etiquette>
        {
            Items = response.Items.Select(MapToModel),
            TotalCount = response.TotalCount,
        };
    }

    private static string BuildFilterQueryString(EtiquetteFilterRequest request)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);

        if (request.DateDebut.HasValue)
            parameters["dateDebut"] = request.DateDebut.Value.ToString("yyyy-MM-dd");

        if (request.DateFin.HasValue)
            parameters["dateFin"] = request.DateFin.Value.ToString("yyyy-MM-dd");

        if (request.Statut.HasValue)
            parameters["statut"] = Convert.ToString(request.Statut);

        if (!string.IsNullOrEmpty(request.N_Sequence))
            parameters["n_Sequence"] = request.N_Sequence;

        if (!string.IsNullOrEmpty(request.N_Commande))
            parameters["n_Commande"] = request.N_Commande;

        if (!string.IsNullOrEmpty(request.Cnuf))
            parameters["cnuf"] = request.Cnuf;

        parameters["PageNumber"] = request.PageNumber.ToString();
        parameters["PageSize"] = request.PageSize.ToString();

        return "?" + parameters.ToString();
    }

    private static async Task<PaginatedResult<EtiquetteDto>> FetchPaginatedEtiquetteDTOs(
        string queryString
    )
    {
        try
        {
            var response = await AuthenticatedHttpClient.GetAsync("Etiquette/filter" + queryString);
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

    private Etiquette MapToModel(EtiquetteDto dto)
    {
        var lignesFacture =
            dto.LigneFactureDTOs?.Select(lf => new LigneFacture
                {
                    Id = 0, // fix that tmrw urgent
                    Taux = lf.Taux,
                    MontantHT = lf.MontantHT,
                })
                .ToList() ?? new List<LigneFacture>();

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

    private static string MapIntMotif(int motif) =>
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
