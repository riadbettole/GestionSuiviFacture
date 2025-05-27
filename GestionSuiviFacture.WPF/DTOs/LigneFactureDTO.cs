using System.Text.Json.Serialization;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class LigneFactureDTO
    {
        public int id_LigneFacture { get; set; }
        [JsonPropertyName("mnHT")]
        public double montant_HT { get; set; }
        public double montant_TVA { get; set; }
        public double montant_TTC { get; set; }

        [JsonPropertyName("taux")]
        public int taux { get; set; }
    }
}
