using System.Text.Json.Serialization;

public class CommandeDTO
{
    public string? Id { get; set; }

    public string? LibelleFournisseur { get; set; }
    public string? LibelleSite { get; set; }
    public string? Cnuf { get; set; }
    public string? Groupe { get; set; }
    public string? Rayon { get; set; }
    public string? NCommande { get; set; }

    public DateTime DateCommande { get; set; }

    public double MontantBRV { get; set; }

    public DateTime DateEcheance { get; set; }


    [JsonPropertyName("bonsLivraison")]
    public BonLivraisonWrapper? BonsLivraison { get; set; }

}

public class BonLivraisonWrapper
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("$values")]
    public IEnumerable<BonDeLivraisonDTO>? Values { get; set; }
}