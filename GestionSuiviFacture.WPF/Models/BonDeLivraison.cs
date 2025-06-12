namespace GestionSuiviFacture.WPF.Models;

public class BonDeLivraison
{
    public string? NumeroLivraison { get; set; }
    public DateTime DateReception { get; set; }
    public double MontantTTC { get; set; }

    public BonDeLivraison(string? numeroLivraison, DateTime dateReception, double montantTTC)
    {
        NumeroLivraison = numeroLivraison;
        DateReception = dateReception;
        MontantTTC = montantTTC;
    }
}
