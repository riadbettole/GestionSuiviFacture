using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.ViewModels.Facture;
using System.Collections.ObjectModel;

namespace GestionSuiviFacture.WPF.Services
{
    class SaisieService
    {
        public Commande _commande;
        public IEnumerable<BonDeLivraison> _bonDeLivraison;
        public SaisieService() 
        {
            _commande = new Commande(
                "BIMBO MOROCCO SARL",
                "51580",
                "Marjane Hay Riad",
                90,
                3525.85,
                DateTime.Now,
                DateTime.Now.AddMonths(1)
            );
            _bonDeLivraison = 
            [
                new BonDeLivraison
                {
                    NumDeLivraison = "99301912",
                    MontantTTC = 1250.00,
                    DateReception = DateTime.Now,
                },
                new BonDeLivraison
                {
                    NumDeLivraison = "99302035",
                    MontantTTC = 3750.25,
                    DateReception = DateTime.Now.AddDays(-3)
                },
                new BonDeLivraison
                {
                    NumDeLivraison = "99303126",
                    MontantTTC = 2485.75,
                    DateReception = DateTime.Now.AddDays(-5)
                }
            ]
            ;
        }
        public Task<Commande> GetCommande()
        {
            return Task.FromResult( _commande );
        }
        public Task<IEnumerable<BonDeLivraison>> GetBonLivraison()
        {
            return Task.FromResult(_bonDeLivraison);
        }
    }
}
