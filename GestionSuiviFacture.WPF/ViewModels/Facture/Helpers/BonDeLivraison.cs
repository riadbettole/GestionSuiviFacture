using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels.Facture
{
    public partial class BonDeLivraison : ObservableObject
    {
        [ObservableProperty]
        private string numDeLivraison;

        [ObservableProperty]
        private double montantTTC;

        [ObservableProperty] private DateTime dateReception;

    }
}
