namespace GestionSuiviFacture.WPF.DTOs;

public class BonDeLivraisonDto
{
    public string? Id { get; set; }
    public string? NumeroLivraison { get; set; }
    public DateTime DateReception { get; set; }
    public double MontantTTC { get; set; }
}
