using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class TaxDetail : ObservableObject
    {
        [ObservableProperty]
        private double _tauxPercentage;

        public TaxDetail(double tauxPercentage, double montantHT)
        {
            TauxPercentage = tauxPercentage;
            MontantHT = montantHT;
            calculTVA();
        }

        [ObservableProperty]
        private double _montantHT;

        [ObservableProperty]
        private double _montantTVA;



        
        public void calculTVA()
        {
            MontantTVA = MontantHT * TauxPercentage / 100;
        }
    }

    public partial class BonDeLivraison : ObservableObject
    {
        [ObservableProperty]
        private string numDeLivraison;

        [ObservableProperty]
        private double montantTTC;
    }

    public partial class InfoSaisieFacture : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<TaxDetail> ligneFacture;

        [ObservableProperty] private string numFacture;
        [ObservableProperty] private double montantTTC;

        [ObservableProperty] private double totalTVA = 0f;
        [ObservableProperty] private double totalHT = 0f;
        [ObservableProperty] private double totalTTC = 0f;

        public InfoSaisieFacture()
        {
            LigneFacture = new ObservableCollection<TaxDetail>();
        }

        public void UpdateTotals(TaxDetail taxDetail)
        {
            TotalTVA += taxDetail.MontantTVA;
            TotalHT += taxDetail.MontantHT;
            TotalTTC = TotalTVA + TotalHT;
        }

        public void AddTax(TaxDetail taxDetail)
        {
            LigneFacture.Add(taxDetail);
            UpdateTotals(taxDetail);
        }
    }

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


        [ObservableProperty] private string _statut = "AUCUN";

        public FactureViewModel()
        {
            _saisieService = new SaisieService();
            SaisieFacture = new InfoSaisieFacture();
            BonDeLivraisons = new ObservableCollection<BonDeLivraison>()
            {
                new BonDeLivraison
                {
                    NumDeLivraison = "99301912",
                    MontantTTC = 1250.00
                },
                new BonDeLivraison
                {
                    NumDeLivraison = "99302035",
                    MontantTTC = 3750.25
                },
                new BonDeLivraison
                {
                    NumDeLivraison = "99303126",
                    MontantTTC = 2485.75
                }
            };
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

        [RelayCommand]
        private async Task FindCommande(String id)
        {
            Commande commande = await _saisieService.GetCommande();
            UpdateCommande(commande);
        }
        public void Dispose()
        {
        }



        private void ShowPopup(FactureViewModel vm)
        {
            AlertePopup.Show(vm);
        }
        [RelayCommand] private void ClosePopup() => AlertePopup.Close();

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
