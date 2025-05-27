using GestionSuiviFacture.WPF.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CommandeDTO
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("libelleFournisseur")]
    public string? LibelleFournisseur { get; set; }

    [JsonPropertyName("cnuf")]
    public string? Cnuf { get; set; }

    [JsonPropertyName("groupe")]
    public string? groupe { get; set; }
    [JsonPropertyName("rayon")]
    public string? Rayon { get; set; }
    [JsonPropertyName("n_Commande")]
    public string? n_Commande { get; set; }

    [JsonPropertyName("date_Commande")]
    public DateTime date_Commande { get; set; }

    [JsonPropertyName("montantBRV")]
    public double MontantBRV { get; set; }

    [JsonPropertyName("date_Echeance")]
    public DateTime DateEcheance { get; set; }

    [JsonPropertyName("bonsLivraison")]
    public BonLivraisonWrapper? BonsLivraison { get; set; }

}

public class BonLivraisonWrapper
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("$values")]
    public List<BonDeLivraisonDTO>? Values { get; set; }
}