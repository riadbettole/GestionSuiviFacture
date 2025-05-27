using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.ViewModels.Facture;

namespace GestionSuiviFacture.WPF.ViewModels
{

    public partial class FactureViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty] 
        private PopupManager<FactureViewModel> alertePopup = new();

        [ObservableProperty] 
        private InfoSaisieFacture _saisieFacture;

        [ObservableProperty]
        private ObservableCollection<BonDeLivraison> bonDeLivraisons;

        [ObservableProperty]
        private CommandeViewModel _commande;
        private readonly SaisieService _saisieService;

        [ObservableProperty] private double _montantTotal = 0;
        [ObservableProperty] private string _statut = "AUCUN";

        public FactureViewModel()
        {
            _saisieService = new SaisieService();
            SaisieFacture = new InfoSaisieFacture();
            BonDeLivraisons = new ObservableCollection<BonDeLivraison>();
            CleanUpCommande();
        }

        private void UpdateMontantTotal()
        {
            double total = 0;

            foreach(BonDeLivraison bl in BonDeLivraisons)
            {
                total += bl.MontantTTC;
            }

            MontantTotal = total;
        }

        [RelayCommand]
        private async Task FindCommande(String id)
        {
            Commande commande = await _saisieService.GetCommande();
            IEnumerable<BonDeLivraison> bonDeLivraison = await _saisieService.GetBonLivraison();
            UpdateCommande(commande);
            UpdateBonDeLivraison(bonDeLivraison);
            UpdateMontantTotal();
            CheckAlert();
        }

        private void UpdateBonDeLivraison(IEnumerable<BonDeLivraison> bonDeLivraison)
        {
            BonDeLivraisons = new ObservableCollection<BonDeLivraison>(bonDeLivraison);
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

            foreach (BonDeLivraison bl in BonDeLivraisons)
            {
                if (SaisieFacture.DateFacture > bl.DateReception.AddDays(6))
                {
                    alertShouldPop = true;
                    dates += bl.DateReception.ToShortDateString().ToString() + ", ";
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
            AlertePopup.Show( null, title,  message,  color, dates);
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
                0,           // Rayon
                0,           // MontantTTC
                DateTime.MinValue,  // DateCommande
                DateTime.MinValue   // DateEcheance
            );

            BonDeLivraisons.Clear();
            Commande = new CommandeViewModel(emptyCommande);
        }


        internal void UpdateStatus()
        {
            int difference = 20;
            //string statut = "";
            if(SaisieFacture.MontantTTC > Commande.MontantTTC + difference)
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
    }
}
