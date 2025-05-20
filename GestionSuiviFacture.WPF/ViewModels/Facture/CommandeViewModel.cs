using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public class CommandeViewModel : ObservableObject, IDisposable
    {
        private readonly Commande _commande;

        public CommandeViewModel(Commande commande)
        {
            _commande = commande;
        }

        public string NomFournisseur => _commande.NomFournisseur;
        public string CNUF => _commande.CNUF;
        public string Rayon => _commande.Rayon.ToString();
        public double MontantTTC => _commande.MontantTTC;
        public string DateCommande => _commande.DateCommande.ToString("dd/MM/yyyy");
        public string DateReception => _commande.DateReception.ToString("dd/MM/yyyy");
        public void Dispose()
        {
        }
    }
}
