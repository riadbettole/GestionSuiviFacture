using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public class EtiquetteViewModel : ObservableObject, IDisposable
    {
        private readonly Etiquette _etiquette;

        // Fields from the Etiquette model

        // Informations Generales
        public string Magasin => _etiquette.Magasin;
        public string NumReception => _etiquette.NumReception;
        public string NumCommande => _etiquette.NumCommande;
        public string NumSequence => _etiquette.NumSequence;
        public string NumFacture => _etiquette.NumFacture;
        public string Cnuf => _etiquette.Cnuf;

        // Informations Temporels
        public string DateTraitement => _etiquette.DateTraitement.ToString("dd/MM/yyyy");
        public string DateReception => _etiquette.DateReception.ToString("dd/MM/yyyy");
        public string DateCommande => _etiquette.DateCommande.ToString("dd/MM/yyyy");

        // Informations Financieres
        public string MontantBF => _etiquette.MontantBF.ToString("C2");
        public string MontantFacture => _etiquette.MontantFacture.ToString("C2"); 

        // Informations Utilisateur
        public string Utilisateur => _etiquette.Utilisateur;
        public string UtilisateurAnnule => _etiquette.UtilisateurAnnule;
        public string MotifAnnulation => _etiquette.MotifAnnulation;
        public string Description => _etiquette.Description;

        // Informations Lieu
        public string LibelleSite => _etiquette.LibelleSite;
        public string GroupeSite => _etiquette.GroupeSite;

        // Enum for Status
        public string Status => EnumToString(_etiquette.Status);

        public string Fournisseur => _etiquette.Fournisseur;
        public EtiquetteViewModel(Etiquette etiquette)
        {
            _etiquette = etiquette;
        }

        // Convert enum to string
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
                default:
                    return "Unknown";
            }
        }

        public void Dispose()
        {
            // Implement necessary cleanup
        }
    }

}
