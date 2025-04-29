namespace GestionSuiviFacture.API.Models
{
    public class Etiquette
    {

        public string? NumSequence { get;}
        public string? Fournisseur { get;}
        public DateTime DateTraitement { get; }
        public StatusEtiquette Status { get;}
        public string? Utilisateur { get; }
        public Etiquette(string? numSequence, string? fournisseur, DateTime dateTraitement, StatusEtiquette status, string? utilisateur)
        {
            NumSequence = numSequence;
            Fournisseur = fournisseur;
            DateTraitement = dateTraitement;
            Status = status;
            Utilisateur = utilisateur;
        }
    }

    public enum StatusEtiquette
    {
        OK,
        NOK,
        ANNULE
    }
}
