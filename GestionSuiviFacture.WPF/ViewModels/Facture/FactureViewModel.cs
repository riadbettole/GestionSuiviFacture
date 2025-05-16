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
        private string tauxPercentage;

        [ObservableProperty]
        private string montantHT;

        [ObservableProperty]
        private string montantTVA;
    }

    public partial class BonDeLivraison : ObservableObject
    {
        [ObservableProperty]
        private string numDeLivraison;

        [ObservableProperty]
        private string montantTTC;
    }
    public partial class FactureViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private ObservableCollection<TaxDetail> taxDetails;
        
        [ObservableProperty]
        private ObservableCollection<BonDeLivraison> bonDeLivraisons;

        [ObservableProperty]
        private CommandeViewModel _commande;
        private readonly SaisieService _saisieService;

        public FactureViewModel()
        {
            _saisieService = new SaisieService();
            TaxDetails = new ObservableCollection<TaxDetail>
        {
            new TaxDetail
            {
                TauxPercentage = "0",
                MontantHT = "0,00 DH",
                MontantTVA = "73.20 DH"
            },
            new TaxDetail
            {
                TauxPercentage = "10",
                MontantHT = "100,00 DH",
                MontantTVA = "10.00 DH"
            }
            };

            BonDeLivraisons = new ObservableCollection<BonDeLivraison>
            {
                new BonDeLivraison
                {
                    NumDeLivraison = "99301912",
                    MontantTTC = "1,250.00 DH"
                },
                new BonDeLivraison
                {
                    NumDeLivraison = "99302035",
                    MontantTTC = "3,750.25 DH"
                },
                new BonDeLivraison
                {
                    NumDeLivraison = "99303126",
                    MontantTTC = "2,485.75 DH"
                }
            };
        }

        private void UpdateCommande(Commande commande)
        {
            Commande = new CommandeViewModel(commande);
        }

        [RelayCommand]
        private void AddTaxDetail(TaxDetail detail)
        {
            TaxDetails.Add(detail);
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
    }
}
