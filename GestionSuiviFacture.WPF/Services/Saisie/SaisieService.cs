using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services
{
    class SaisieService
    {
        public Commande _commande;
        public SaisieService() 
        {
            _commande = new Commande(
                "BIMBO MOROCCO SARL",
                "51580",
                90,
                3525.85,
                DateTime.Now,
                DateTime.Now.AddMonths(1)
            );
        }
        public Task<Commande> GetCommande()
        {
            return Task.FromResult( _commande );
        }
    }
}
