namespace GestionSuiviFacture.WPF.Models
{
    public record Etiquette(

        DateTime? DateTraitement,
        string? NumSequence,
        int? Status,

        DateTime? DateCommande,
        double MontantBRV,
        string? NumCommande,
        string? Fournisseur,
        string? LibelleSite,
        string? GroupeSite,
        string? Magasin,
        string? Cnuf,

        DateTime? DateFacture,
        string? NumFacture,
        double? MontantFacture,
        List<LigneFacture>? LignesFacture,

        DateTime? DateAnnulation,
        string? UtilisateurAnnule,
        string? MotifAnnulation,
        string? DescriptionAnnulation,

        string? Utilisateur
    );
}
