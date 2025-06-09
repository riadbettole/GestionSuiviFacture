using GestionSuiviFacture.WPF.DTOs.FromBackend;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class FactureFrontendDTO
    {
        public int id_Facture { get; set; }
        public string? n_Facture { get; set; }
        public DateTime date_Facture { get; set; }
        public decimal montant_Facture { get; set; }

        public List<EtiquetteDTO>? etiquettes { get; set; } // to remove potentially
        public List<LigneFactureFrontendDTO>? lignesFactures { get; set; }
    }
}
