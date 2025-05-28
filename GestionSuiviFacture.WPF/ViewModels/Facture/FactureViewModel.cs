using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
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

        [ObservableProperty] private double _montantTotal = 0;
        [ObservableProperty] private string _statut = "AUCUN";

        public FactureViewModel()
        {
            _commandeService = new CommandeService();
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
            Commande commande = await _commandeService.GetCommandeByFilterAsync(SaisieFacture.DateFacture, SaisieFacture.NumSite, SaisieFacture.NumCommande);

            if(commande == null)
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

        private void NoCommandFound()
        {
    
           
            string title = "COMMANDE OU SITE INTROUVABLE";
            string message = "Ce numero de commande ou site est introuvable. Vérifiez avant de continuer.";
            string  color = "#FF5C5C";
                string dates = string.Empty;

            NotFoundPopup.Show(this, title, message, color, dates);
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

        public void Dispose()
        {
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
                "-----",          // CNUF
                "---",           // Rayon
                0,           // MontantTTC
                DateTime.MinValue,  // DateCommande
                DateTime.MinValue,   // DateEcheance
                new List<BonDeLivraison>()
            );

            BonDeLivraisons.Clear();
            Commande = new CommandeViewModel(emptyCommande);
        }


        internal void UpdateStatus()
        {
            int difference = 20;
            //string statut = "";
            if (SaisieFacture.MontantTTC > Commande.MontantTTC + difference)
            {
                Statut = "NOK";
                return;
            }

            if (SaisieFacture.TotalTTC > Commande.MontantTTC + difference)
            {
                Statut = "NOK";
            }
            else
            {
                Statut = "OK";
            }

           
            //Statut = statut;
        }

        [RelayCommand]
        public void CloseNotFound() => NotFoundPopup.Close();
    }
}