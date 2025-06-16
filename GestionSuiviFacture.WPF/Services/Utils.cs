using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using System.Web;

namespace GestionSuiviFacture.WPF.Services;

public static class EtiquetteUtils
{
    public static string BuildFilterQueryString(EtiquetteFilterRequest request)
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

    public static Etiquette MapToModel(EtiquetteDto dto)
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

    public static string MapIntMotif(int motif) =>
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


public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

public class EtiquetteFilterRequest
{
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int? Statut { get; set; }
    public string? N_Sequence { get; set; }
    public string? N_Commande { get; set; }
    public string? Cnuf { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;
}


public class CommandeUtils
{
    public static string BuildFilterQueryString(string? numSite = null, string? numCommande = null)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrEmpty(numSite))
            parameters["n_site"] = numSite;
        if (!string.IsNullOrEmpty(numCommande))
            parameters["n_commande"] = numCommande;

        return parameters.Count > 0 ? "?" + parameters.ToString() : "";
    }

    public static Commande MapToModel(CommandeDto dto)
    {
        return new Commande(
            NomFournisseur: dto.LibelleFournisseur,
            CNUF: dto.Cnuf,
            Site: dto.LibelleSite,
            Rayon: dto.Rayon,
            Groupe: dto.Groupe,
            MontantTTC: dto.MontantBRV,
            DateCommande: dto.DateCommande,
            DateEcheance: dto.DateEcheance,
            BonDeLivraison: MapToModelBL(
                dto.BonsLivraison?.Values ?? Enumerable.Empty<BonDeLivraisonDto>()
            )
        );
    }

    public static List<BonDeLivraison> MapToModelBL(IEnumerable<BonDeLivraisonDto> dtos)
    {
        List<BonDeLivraison> bls = [];

        foreach (BonDeLivraisonDto dto in dtos)
        {
            bls.Add(new BonDeLivraison(dto.NumeroLivraison, dto.DateReception, dto.MontantTTC));
        }

        return bls;
    }

    public static IEnumerable<BonDeLivraisonDto> MapToBonDeLivraison(CommandeDto dto)
    {
        if (dto.BonsLivraison?.Values == null)
            return Enumerable.Empty<BonDeLivraisonDto>();

        return dto
            .BonsLivraison.Values.Select(blDto => new BonDeLivraisonDto
            {
                NumeroLivraison = blDto.NumeroLivraison,
                DateReception = blDto.DateReception,
                MontantTTC = blDto.MontantTTC,
            })
            .ToList();
    }
}