using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public class EtiquetteViewModel : ObservableObject
    {
        private readonly Etiquette _etiquette;

        public string NumSequence => _etiquette.NumSequence;
        public string Fournisseur => _etiquette.Fournisseur;
        public string DateTraitement => _etiquette.DateTraitement.ToString("dd/MM/yyyy");
        public string Status => EnumToString(_etiquette.Status);
        public string Utilisateur => _etiquette.Utilisateur;

        public EtiquetteViewModel(Etiquette etiquette)
        {
            _etiquette = etiquette;
        }

        public string EnumToString(StatusEtiquette status)
        {
            switch (status)
            {
                case StatusEtiquette.OK:
                    return "OK";
                case StatusEtiquette.NOK:
                    return "NOK";
                case StatusEtiquette.ANNULE:
                    return "ANNULE";
            }
            return "";
        }
    }
}
