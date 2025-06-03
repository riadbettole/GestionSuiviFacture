using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.Views;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string currentUser = AuthService.Username; // Get from your auth service

        [ObservableProperty]
        private string currentPageTitle = "Tableau de bord";

        [ObservableProperty]
        private string currentPageSubtitle = "Bienvenue dans le système de gestion";

        [RelayCommand]
        private void Disconnect()
        {
            (App.Current as App)?.OnLogout();
        }
        public NavigationService Navigation { get; }

        public MainViewModel()
        {
            Navigation = new NavigationService();
        }

        [RelayCommand]
        public void NavigateToFacture() 
        {
            CurrentPageTitle = "Facture Normales";
            CurrentPageSubtitle = "Gestion des factures standard";
            SetActivePage("Facture");
            Navigation.NavigateTo(new FactureViewModel());
        }

        //[RelayCommand]
        //public void NavigateToFactureEmballage() 
        //{
        //    CurrentPageTitle = "Facture Emballage";
        //    CurrentPageSubtitle = "Gestion des factures d'emballages";
        //    Navigation.NavigateTo(new FactureEmballageViewModel());
        //} 

        [RelayCommand]
        public void NavigateToConsultation()
        {
            CurrentPageTitle = "Consultations";
            CurrentPageSubtitle = "Recherche et consultation d'étiquettes";
            SetActivePage("Consultation");
            Navigation.NavigateTo(new ConsultationViewModel());
        }

        [ObservableProperty]
        private string activePage;

        public void SetActivePage(string page)
        {
            ActivePage = page;
        }



        public void Dispose()
        {
            if (Navigation.CurrentViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }
    }
}
