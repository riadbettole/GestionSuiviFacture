using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;
using System.Collections.ObjectModel;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public class EtiquetteViewModel : ObservableObject, IDisposable
    {
        public readonly Etiquette _etiquette;

        // Fields from the Etiquette model

        // Informations Generales
        public string Magasin => _etiquette.Magasin;
        public string NumCommande => _etiquette.NumCommande;
        public string NumSequence => _etiquette.NumSequence;
        public string NumFacture => _etiquette.NumFacture;
        public string Cnuf => _etiquette.Cnuf;

        // Informations Temporels
        public DateTime DateTraitement => _etiquette.DateTraitement;
        public DateTime? DateCommande => _etiquette.DateCommande;
        public DateTime? DateFacture => _etiquette.DateFacture;

        // Informations Financieres
        public Double MontantBRV => _etiquette.MontantBRV;
        public Double MontantFacture => _etiquette.MontantFacture; 

        // Informations Utilisateur
        public string Utilisateur => _etiquette.Utilisateur;
        public string? UtilisateurAnnule => _etiquette.UtilisateurAnnule;
        public string? MotifAnnulation => _etiquette.MotifAnnulation;
        public string? Description => _etiquette.DescriptionAnnulation;
        public DateTime? DateAnnulation => _etiquette.DateAnnulation;

        // Informations Lieu
        public string LibelleSite => _etiquette.LibelleSite;
        public string GroupeSite => _etiquette.GroupeSite;

        // Enum for Status
        public string Status => EnumToString(_etiquette.Status);

        public string Fournisseur => _etiquette.Fournisseur;

        public ObservableCollection<LigneFactureViewModel> LignesFacture { get; }

        public EtiquetteViewModel(Etiquette etiquette)
        {
            _etiquette = etiquette;

            LignesFacture = new ObservableCollection<LigneFactureViewModel>(
        etiquette.LignesFacture.Select(lf => new LigneFactureViewModel(lf))
    );
        }

        // Convert enum to string
        public string EnumToString(StatusEtiquette status)
        {
            switch (status)
            {
                case StatusEtiquette.OK:
                    return "OK";
                case StatusEtiquette.NOK:
                    return "NOK";
                case StatusEtiquette.ANNULE:
                    return "ANNULE";
                default:
                    return "AUCUN";
            }
        }

        public void Dispose()
        {
            // Implement necessary cleanup
        }
    }

    public class LigneFactureViewModel
    {
        LigneFacture _ligneFacture;

        public decimal Taux => _ligneFacture.Taux;
        public decimal MontantHT => _ligneFacture.MontantHT;
        public decimal MontantTVA => _ligneFacture.MontantHT + _ligneFacture.MontantHT * _ligneFacture.Taux / 100;
        public LigneFactureViewModel(LigneFacture ligneFacture)
        {
            _ligneFacture = ligneFacture;
        }
    }


}
