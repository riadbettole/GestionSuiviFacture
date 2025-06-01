using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string currentUser = "Admin User"; // Get from your auth service

        [ObservableProperty]
        private string currentPageTitle = "Tableau de bord";

        [ObservableProperty]
        private string currentPageSubtitle = "Bienvenue dans le système de gestion";

        [RelayCommand]
        private void Disconnect()
        {
            // Clear user session
            // Navigate back to login
            // Close current window and show login
        }
        public NavigationService Navigation { get; }

        public MainViewModel()
        {
            Navigation = new NavigationService();
        }

        [RelayCommand]
        public void NavigateToFacture() => Navigation.NavigateTo(new FactureViewModel());

        [RelayCommand]
        public void NavigateToFactureEmballage() => Navigation.NavigateTo(new FactureEmballageViewModel());

        [RelayCommand]
        public void NavigateToConsultation() => Navigation.NavigateTo(new ConsultationViewModel());

        public void Dispose()
        {
            if (Navigation.CurrentViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }
    }
}
