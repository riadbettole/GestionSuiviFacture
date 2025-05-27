namespace GestionSuiviFacture.WPF.Models
{
    public record Commande(
    string? NomFournisseur,
    string? CNUF,
    string? Site,
    int Rayon,
    double MontantTTC,
    DateTime DateCommande,
    DateTime DateEcheance
);
}
