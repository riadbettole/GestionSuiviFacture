namespace GestionSuiviFacture.WPF.DTOs
{
    public class SiteDTO
    {
        public int id_Site { get; set; }
        public string? n_Site { get; set; }
        public string? libelle_Site { get; set; }
        public List<EtiquetteDTO>? etiquettes { get; set; }
    }
}
