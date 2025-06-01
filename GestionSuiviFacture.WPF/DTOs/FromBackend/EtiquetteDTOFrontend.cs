namespace GestionSuiviFacture.WPF.DTOs
{
    public class EtiquetteFrontendDTO
    {
        public string? NumCommande { get; set; }

        // INFO FOURNISSEURS
        public string? NSite { get; set; }
        public string? Site { get; set; }
        public string? LibelleFournisseur { get; set; }
        public string? Cnuf { get; set; }


        public DateTime? DateCommande { get; set; }
        public DateTime? DateEcheance { get; set; }
        public DateTime? DateFacture { get; set; }

        public string? Rayon { get; set; }
        public string? Groupe { get; set; }

        public double? MontantBRV { get; set; }

        // Facture Detail
        public string? Statut { get; set; }
        public string? NumFacture { get; set; }
        public double? MontantTTCFacture { get; set; }
        public IEnumerable<LigneFactureDTO>? LigneFactureDTOs { get; set; }

        // Utilisateur
        public int? Utilisateur { get; set; }
    }
}
