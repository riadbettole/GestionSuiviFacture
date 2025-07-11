﻿namespace GestionSuiviFacture.WPF.DTOs;

public class EtiquetteDto
{
    public string? NumCommande { get; set; }

    public string? NSite { get; set; }
    public string? Site { get; set; }
    public string? LibelleFournisseur { get; set; }
    public string? Cnuf { get; set; }

    public DateTime? DateCommande { get; set; }
    public DateTime? DateEcheance { get; set; }
    public DateTime? DateFacture { get; set; }
    public DateTime? DateTraitement { get; set; }

    public string? Rayon { get; set; }
    public string? Groupe { get; set; }

    public double? MontantBRV { get; set; }

    public int? Statut { get; set; }
    public string? NumFacture { get; set; }
    public string? NSequence { get; set; }
    public double? MontantTTCFacture { get; set; }
    public IEnumerable<LigneFactureDto>? LigneFactureDTOs { get; set; }

    public int? UtilisateurId { get; set; }
    public string? UtilisateurNom { get; set; }

    public string? UtilisateurNomAnnulation { get; set; }
    public DateTime? DateAnnulation { get; set; }
    public string? DescriptionAnnulation { get; set; }
    public int? MotifAnnulation { get; set; }
}
