using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels.Facture
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

        [ObservableProperty] private double _montantHT;

        [ObservableProperty] private double _montantTVA;



        public void calculTVA()
        {
            MontantTVA = MontantHT * TauxPercentage / 100;
        }
    }
}
