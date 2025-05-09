namespace GestionSuiviFacture.WPF.Models
{
    public record Etiquette(
    string? Magasin,
    string? Cnuf,
    string? NumSequence,
    string? NumFacture,
    string? NumReception,
    string? NumCommande,
    DateTime DateTraitement,
    DateTime DateReception,
    DateTime DateCommande,
    DateTime DateRapprochement,
    StatusEtiquette Status,
    string? Fournisseur,
    double MontantBF,
    double MontantFacture,
    string? Utilisateur,
    string? UtilisateurAnnule,
    string? MotifAnnulation,
    string? Description,
    string? LibelleSite,
    string? GroupeSite
);
    public class EtiquetteOld
    {
        public string? Magasin{ get;}
        public string? Cnuf { get; }
        public string? NumSequence { get;}
        public string? NumFacture { get;}
        public string? NumReception { get;}
        public string? NumCommande { get; }

        public DateTime DateTraitement { get; }
        public DateTime DateReception { get; }
        public DateTime DateCommande { get; }
        public DateTime DateRapprochement{ get; }

        public StatusEtiquette Status { get;}
        public string? Fournisseur { get;}

        public double MontantBF { get; }
        public double MontantFacture { get; }

        public string? Utilisateur { get; }
        public string? UtilisateurAnnule { get; }
        public string? MotifAnnulation { get; }
        public string? Description { get; }
    
        public string? LibelleSite { get; }
        public string? GroupeSite { get; }

        public EtiquetteOld(string? magasin, string? cnuf, string? numSequence, string? numFacture, string? numReception, string? numCommande, DateTime dateTraitement, DateTime dateReception, DateTime dateCommande, DateTime dateRapprochement, StatusEtiquette status, string? fournisseur, double montantBF, double montantFacture, string? utilisateur, string? utilisateurAnnule, string? motifAnnulation, string? description, string? libelleSite, string? groupeSite)
        {
            Magasin = magasin;
            Cnuf = cnuf;
            NumSequence = numSequence;
            NumFacture = numFacture;
            NumReception = numReception;
            NumCommande = numCommande;
            DateTraitement = dateTraitement;
            DateReception = dateReception;
            DateCommande = dateCommande;
            DateRapprochement = dateRapprochement;
            Status = status;
            Fournisseur = fournisseur;
            MontantBF = montantBF;
            MontantFacture = montantFacture;
            Utilisateur = utilisateur;
            UtilisateurAnnule = utilisateurAnnule;
            MotifAnnulation = motifAnnulation;
            Description = description;
            LibelleSite = libelleSite;
            GroupeSite = groupeSite;
        }
    }

    public enum StatusEtiquette
    {
        OK,
        NOK,
        ANNULE
    }
}
