using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSuiviFacture.WPF.DTOs
{
    public class EtiquetteDTO
    {
        public int id_Etiquette { get; set; }
        public string n_sequence { get; set; }
        public DateTime date_Traitement { get; set; }
        public int statut { get; set; }
        public int? id_LigneFacture { get; set; }
        public int? id_Facture { get; set; }
        public FactureDTO facture { get; set; }
        public int? id_Reception { get; set; }
        public ReceptionDTO reception { get; set; }
        public int? id_Commande { get; set; }
        public CommandeDTO commande { get; set; }
        public int? id_Fournisseur { get; set; }
        public FournisseurDTO fournisseur { get; set; }
        public int? id_Site { get; set; }
        public SiteDTO site { get; set; }
    }
}
