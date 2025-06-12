namespace GestionSuiviFacture.WPF.Models;

public record Commande(
    string? NomFournisseur,
    string? CNUF,
    string? Site,
    string? Rayon,
    string? Groupe,
    double? MontantTTC,
    DateTime? DateCommande,
    DateTime? DateEcheance,
    IEnumerable<BonDeLivraison> BonDeLivraison
);
