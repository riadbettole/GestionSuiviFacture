using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class CommandeDTO
    {
        public int id_Commande { get; set; }
        public string n_Commande { get; set; }
        public DateTime date_Commande { get; set; }
        public string groupe { get; set; }
        public string rayon { get; set; }
        public List<EtiquetteDTO> etiquettes { get; set; }
        public string n_Site { get; set; }
    }
}
