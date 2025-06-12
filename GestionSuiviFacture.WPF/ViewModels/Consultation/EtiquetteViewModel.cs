using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.ViewModels;

public class EtiquetteViewModel : ObservableObject
{
    public readonly Etiquette _etiquette;

    public string? Magasin => _etiquette.Magasin;
    public string? NumCommande => _etiquette.NumCommande;
    public string? NumSequence => _etiquette.NumSequence;
    public string? NumFacture => _etiquette.NumFacture;
    public string? Cnuf => _etiquette.Cnuf;

    public DateTime? DateTraitement => _etiquette.DateTraitement;
    public DateTime? DateCommande => _etiquette.DateCommande;
    public DateTime? DateFacture => _etiquette.DateFacture;

    public Double? MontantBRV => _etiquette.MontantBRV;
    public Double? MontantFacture => _etiquette.MontantFacture;

    public string? Utilisateur => _etiquette.Utilisateur;
    public string? UtilisateurLetter => _etiquette.Utilisateur?.Substring(0, 1).ToUpper();
    public string? UtilisateurAnnule => _etiquette.UtilisateurAnnule;
    public string? MotifAnnulation => _etiquette.MotifAnnulation;
    public string? Description => _etiquette.DescriptionAnnulation;
    public DateTime? DateAnnulation => _etiquette.DateAnnulation;

    public string? LibelleSite => _etiquette.LibelleSite;
    public string? GroupeSite => _etiquette.GroupeSite;

    public string? Status => EnumToString(_etiquette.Status);

    public string? Fournisseur => _etiquette.Fournisseur;

    public ObservableCollection<LigneFactureViewModel> LignesFacture { get; }

    public EtiquetteViewModel(Etiquette etiquette)
    {
        _etiquette = etiquette;

        LignesFacture = new ObservableCollection<LigneFactureViewModel>(
            (etiquette.LignesFacture ?? Enumerable.Empty<LigneFacture>()).Select(
                lf => new LigneFactureViewModel(lf)
            )
        );
    }

    public static string EnumToString(int? status)
    {
        switch (status)
        {
            case 0:
                return "OK";
            case 1:
                return "NOK";
            case 2:
                return "ANNULE";
            default:
                return "AUCUN";
        }
    }
}

public class LigneFactureViewModel
{
    readonly LigneFacture _ligneFacture;

    public double Taux => _ligneFacture.Taux;
    public double MontantHT => _ligneFacture.MontantHT;
    public double MontantTVA =>
        _ligneFacture.MontantHT + _ligneFacture.MontantHT * _ligneFacture.Taux / 100;

    public LigneFactureViewModel(LigneFacture ligneFacture)
    {
        _ligneFacture = ligneFacture;
    }
}
