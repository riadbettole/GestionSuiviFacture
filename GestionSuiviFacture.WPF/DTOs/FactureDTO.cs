using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class FactureDTO
    {
        public int id_Facture { get; set; }
        public string n_Facture { get; set; }
        public DateTime date_Facture { get; set; }
        public double total_HT { get; set; }
        public double total_TVA { get; set; }
        public double total_TTC { get; set; }
        public List<EtiquetteDTO> etiquettes { get; set; }
        public List<LigneFactureDTO> lignesFactures { get; set; }
    }

}
