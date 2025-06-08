using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.Services.Saisie;
using GestionSuiviFacture.WPF.ViewModels.Facture;
using GestionSuiviFacture.WPF.ViewModels.Facture.Helpers;
using System.Collections.ObjectModel;

namespace GestionSuiviFacture.WPF.ViewModels
{

    public partial class FactureViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private PopupManager<FactureViewModel> alertePopup = new();

        [ObservableProperty]
        private PopupManager<FactureViewModel> notFoundPopup = new();

        [ObservableProperty]
        private InfoSaisieFacture _saisieFacture;

        [ObservableProperty]
        private ObservableCollection<BonDeLivraisonViewModel> bonDeLivraisons;

        [ObservableProperty]
        private CommandeViewModel _commande;
        private readonly CommandeService _commandeService;
        private readonly FactureService _factureService;

        [ObservableProperty] private double _montantTotal = 0;
        [ObservableProperty] private string _statut = "AUCUN";

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isSearching = false;
        public FactureViewModel()
        {
            _commandeService = new CommandeService();
            _factureService = new FactureService();

            SaisieFacture = new InfoSaisieFacture();
            BonDeLivraisons = new ObservableCollection<BonDeLivraisonViewModel>();
            CleanUpCommande();
        }

        private void UpdateMontantTotal()
        {
            double total = 0;

            foreach (BonDeLivraisonViewModel bl in BonDeLivraisons)
            {
                total += bl.montantTTC;
            }

            MontantTotal = total;
        }

        [RelayCommand]
        private async Task FindCommande(String id)
        {
            CleanUpCommande();
            CleanUpSaisie();
            IsSearching = true;
            try
            {
                Commande commande = await _commandeService.GetCommandeByFilterAsync(SaisieFacture.NumSite, SaisieFacture.NumCommande);
                if (commande == null)
                {
                    NoCommandFound();
                    return;
                }
                IEnumerable<BonDeLivraison> bonDeLivraison = commande.BonDeLivraison;
                UpdateCommande(commande);
                UpdateBonDeLivraison(bonDeLivraison);
                UpdateMontantTotal();
                CheckAlert();
            }
            finally
            {
                IsSearching = false;
            }

           
        }

        private void NoCommandFound()
        {
            NotFoundPopup.Show(
                this,
                "COMMANDE OU SITE INTROUVABLE",
                "Ce numero de commande ou site est introuvable. Vérifiez avant de continuer.",
                "#FF5C5C");
        }

        private void UpdateBonDeLivraison(IEnumerable<BonDeLivraison> bonDeLivraison)
        {
            BonDeLivraisons = new ObservableCollection<BonDeLivraisonViewModel>(
                    bonDeLivraison.Select(b => new BonDeLivraisonViewModel(b))
                );
        }


        private void UpdateCommande(Commande commande)
        {
            Commande = new CommandeViewModel(commande);
        }

        [RelayCommand]
        private void AddTaxDetail(TaxDetail taxDetail)
        {
            if (taxDetail is null) return;

            SaisieFacture.AddTax(taxDetail);
        }


        [RelayCommand(CanExecute = nameof(CanTraiter))]
        private async void SaveFacture()
        {
            IsLoading = true;

            try
            {
                EtiquetteFrontendDTO etiquetteDto = MakeEtiquetteDTO();

                await _factureService.PostEtiquetteAsync(etiquetteDto);

                CleanUpSaisie();
                CleanUpCommande();
                CleanFilter();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private EtiquetteFrontendDTO MakeEtiquetteDTO()
        {
            return new EtiquetteFrontendDTO
            {
                NumCommande = SaisieFacture.NumCommande,

                NSite = SaisieFacture.NumSite,
                Site = Commande.Site,
                LibelleFournisseur = Commande.NomFournisseur,
                Cnuf = Commande.CNUF,

                DateCommande = Commande.DateCommande,
                DateEcheance = Commande.DateEcheance,
                DateFacture = SaisieFacture.DateFacture,

                Rayon = Commande.Rayon,
                MontantBRV = Commande.MontantTTC,
                Groupe = Commande.Groupe,

                Statut = Statut,

                NumFacture = SaisieFacture.NumFacture,
                MontantTTCFacture = SaisieFacture.MontantTTC,

                UtilisateurId = AuthService.UserID,

                LigneFactureDTOs = SaisieFacture.LigneFacture.Select(tax => new LigneFactureDTO
                {
                    montant_HT = tax.MontantHT,
                    taux = tax.TauxPercentage
                }),

            };
        }

        private bool CanTraiter() => !SaisieFacture.LigneFacture.Any();

        private void CheckAlert()
        {
            Boolean alertShouldPop = false;
            string title = "FACTURE EN RETARD", color = "#FFCF00", dates = "",
                message = "La facture a un retard de plus de 6 jours. Vérifiez avant de continuer.";

            foreach (BonDeLivraisonViewModel bl in BonDeLivraisons)
            {
                if (SaisieFacture.DateFacture > bl.dateReception.AddDays(6))
                {
                    alertShouldPop = true;
                    dates += bl.dateReception.ToShortDateString().ToString() + ", ";
                }
            }
            dates = dates.TrimEnd(',', ' ');

            if (SaisieFacture.DateFacture > Commande.DateEcheance)
            {
                alertShouldPop = true;
                title = "FACTURE ECHUE";
                message = "La facture a passé la date d'échéance. Vérifiez avant de continuer.";
                color = "#FF5C5C";
                dates = Commande.DateEcheance.ToString();
            }
            dates += ".";

            if (alertShouldPop) ShowPopup(title, message, color, dates);
        }

        private void ShowPopup(string title, string message, string color, string dates)
        {
            AlertePopup.Show(null, title, message, color, dates);
        }
        [RelayCommand] private void ClosePopup() => AlertePopup.Close();

        public void ClosePopupAndClean()
        {
            CleanUpCommande();
            AlertePopup.Close();
        }

        private void CleanUpCommande()
        {
            var emptyCommande = new Commande(
                "-----",          // NomFournisseur
                "-----",          // CNUF
                "-----",          // Site
                "---",           // Rayon
                "-----",          // GROUPE
                0,           // MontantBRV
                null,  // DateCommande
                null,   // DateEcheance
                new List<BonDeLivraison>()
            );
            MontantTotal = 0;

            BonDeLivraisons.Clear();
            Commande = new CommandeViewModel(emptyCommande);
        }

        private void CleanUpSaisie()
        {
            SaisieFacture.NumFacture = "";
            SaisieFacture.MontantTTC = null;

            SaisieFacture.LigneFacture.Clear();
            SaisieFacture.TotalHT = 0;
            SaisieFacture.TotalTVA = 0;
            SaisieFacture.TotalTTC = 0;
            Statut = "AUCUN";
        }

        private void CleanFilter()
        {
            SaisieFacture.NumSite = "";
            SaisieFacture.NumCommande = "";
            SaisieFacture.DateFacture = DateTime.Now;
        }

        internal void UpdateStatus()
        {
            Statut = IsItWorking() ? "OK" : "NOK";
        }

        private bool IsItWorking()
        {
            bool isCommandeValid = Math.Abs(MontantTotal - (Double)Commande.MontantTTC) <= 20;
            bool isSaisie1Valid = Math.Abs(MontantTotal - (Double)SaisieFacture.MontantTTC) <= 20;
            bool isSaisie2Valid = Math.Abs(MontantTotal - SaisieFacture.TotalTTC) <= 20;

            return isCommandeValid && isSaisie1Valid && isSaisie2Valid;
        }

        [RelayCommand]
        public void CloseNotFound() => NotFoundPopup.Close();


        public void Dispose()
        {
        }

    }
}