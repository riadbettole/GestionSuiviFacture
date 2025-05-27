using System.Text.Json.Serialization;

public class BonDeLivraisonDTO
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("numeroLivraison")]
    public string? NumeroLivraison { get; set; }

    [JsonPropertyName("date_Reception")]
    public DateTime DateReception { get; set; }

    [JsonPropertyName("montantTTC")]
    public double MontantTTC { get; set; }

    public BonDeLivraisonDTO(string? numeroLivraison, DateTime dateReception, double montantTTC)
    {
        NumeroLivraison = numeroLivraison;
        DateReception = dateReception;
        MontantTTC = montantTTC;
    }
}