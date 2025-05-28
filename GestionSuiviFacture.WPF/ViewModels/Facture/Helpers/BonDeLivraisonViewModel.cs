using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.ViewModels.Facture
{
    public partial class BonDeLivraisonViewModel : ObservableObject
    {
        private readonly BonDeLivraison _bonDeLivraison;

        public BonDeLivraisonViewModel(BonDeLivraison   bonDeLivraison)
        {
            _bonDeLivraison =  bonDeLivraison;
        }

        public string numDeLivraison => _bonDeLivraison.NumeroLivraison;
        public double montantTTC => _bonDeLivraison.MontantTTC;
        public DateTime dateReception => _bonDeLivraison.DateReception;
    }
}
