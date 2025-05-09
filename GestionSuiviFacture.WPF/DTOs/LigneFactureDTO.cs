using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class LigneFactureDTO
    {
        public int id_LigneFacture { get; set; }
        public double montant_HT { get; set; }
        public double montant_TVA { get; set; }
        public double montant_TTC { get; set; }
        public int id_Facture { get; set; }
        // Add additional properties as needed
    }
}
