namespace GestionSuiviFacture.WPF.Models
{
    public record Etiquette(
    string? Magasin,
    string? Cnuf,
    string? NumSequence,
    string? NumFacture,
    string? NumCommande,
    DateTime DateTraitement,
    DateTime DateCommande,
    DateTime DateFacture,
    StatusEtiquette Status,
    string? Fournisseur,
    double MontantBRV,
    double MontantFacture,
    string? Utilisateur,
    string? UtilisateurAnnule,
    string? MotifAnnulation,
    string? Description,
    string? LibelleSite,
    string? GroupeSite,
    List<LigneFacture> LignesFacture
);
    //public class Etiquette
    //{
    //    public string? Magasin{ get;}
    //    public string? Cnuf { get; }
    //    public string? NumSequence { get;}
    //    public string? NumFacture { get;}
    //    public string? NumCommande { get; }

    //    public DateTime DateTraitement { get; }
    //    public DateTime DateCommande { get; }
    //    public DateTime DateRapprochement{ get; }

    //    public StatusEtiquette Status { get;}
    //    public string? Fournisseur { get;}

    //    public double MontantBF { get; }
    //    public double MontantFacture { get; }

    //    public string? Utilisateur { get; }
    //    public string? UtilisateurAnnule { get; }
    //    public string? MotifAnnulation { get; }
    //    public string? Description { get; }
    
    //    public string? LibelleSite { get; }
    //    public string? GroupeSite { get; }


    //    public EtiquetteOld(string? magasin, string? cnuf, string? numSequence, string? numFacture, string? numCommande, DateTime dateTraitement, DateTime dateCommande, DateTime dateRapprochement, StatusEtiquette status, string? fournisseur, double montantBF, double montantFacture, string? utilisateur, string? utilisateurAnnule, string? motifAnnulation, string? description, string? libelleSite, string? groupeSite)
    //    {
    //        Magasin = magasin;
    //        Cnuf = cnuf;
    //        NumSequence = numSequence;
    //        NumFacture = numFacture;
    //        NumCommande = numCommande;
    //        DateTraitement = dateTraitement;
    //        DateCommande = dateCommande;
    //        DateRapprochement = dateRapprochement;
    //        Status = status;
    //        Fournisseur = fournisseur;
    //        MontantBF = montantBF;
    //        MontantFacture = montantFacture;
    //        Utilisateur = utilisateur;
    //        UtilisateurAnnule = utilisateurAnnule;
    //        MotifAnnulation = motifAnnulation;
    //        Description = description;
    //        LibelleSite = libelleSite;
    //        GroupeSite = groupeSite;
    //    }
    //}

    public enum StatusEtiquette
    {
        OK,
        NOK,
        ANNULE
    }
}
