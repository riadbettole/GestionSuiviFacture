using System.Text.Json.Serialization;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class FactureDTO
    {
        public int id_Facture { get; set; }
        [JsonPropertyName("n_Facture")]
        public string n_Facture { get; set; }
        [JsonPropertyName("date_Facture")]
        public DateTime date_Facture { get; set; }
        [JsonPropertyName("montant_Facture")]
        public double montant_Facture { get; set; }
        public double total_HT { get; set; }
        public double total_TVA { get; set; }
        public double total_TTC { get; set; }

        [JsonPropertyName("statut")]
        public int statut { get; set; }

        [JsonPropertyName("lignes")]
        public List<LigneFactureDTO> lignesFactures { get; set; }
    }

}
