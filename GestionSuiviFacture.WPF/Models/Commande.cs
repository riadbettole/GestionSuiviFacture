namespace GestionSuiviFacture.WPF.Models
{
    public record Commande(
    string? NomFournisseur,
    string? CNUF,
    string? Site,
    string? Rayon,
    double MontantTTC,
    DateTime DateCommande,
    DateTime DateEcheance
);
}
