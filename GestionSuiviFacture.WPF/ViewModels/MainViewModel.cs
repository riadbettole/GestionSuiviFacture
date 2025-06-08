using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string currentUser = AuthService.Username;

        [ObservableProperty]
        private string firstLetterNameUser = AuthService.Username.Substring(0, 1).ToUpper();

        [ObservableProperty]
        private string currentPageTitle = "Tableau de bord";

        [ObservableProperty]
        private string currentPageSubtitle = "Bienvenue dans le système de gestion";

        [ObservableProperty]
        private string connexionStatus;

        [ObservableProperty]
        private string connexionStatusColor;

        [RelayCommand]
        private void Disconnect()
        {
            (App.Current as App)?.OnLogout();
        }

        public NavigationService Navigation { get; }
        private readonly NetworkService _networkService;

        public MainViewModel()
        {
            Navigation = new NavigationService();

            // Initialize network service
            _networkService = new NetworkService(1000); // Check every second
            _networkService.NetworkStatusChanged += OnNetworkStatusChanged;

            // Set initial status
            ConnexionStatus = "Connecté";
            ConnexionStatusColor = "Green";

            // Start monitoring
            _networkService.StartMonitoring();
        }

        private void OnNetworkStatusChanged(object? sender, bool isConnected)
        {
            if (isConnected)
            {
                ConnexionStatus = "Connecté";
                ConnexionStatusColor = "Green";
            }
            else
            {
                ConnexionStatus = "Déconnecté";
                ConnexionStatusColor = "Red";
            }
        }

        [RelayCommand]
        public void NavigateToFacture()
        {
            CurrentPageTitle = "Facture Normales";
            CurrentPageSubtitle = "Gestion des factures standard";
            SetActivePage("Facture");
            Navigation.NavigateTo(new FactureViewModel());
        }

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
            _networkService?.Dispose();

            if (Navigation.CurrentViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }
    }
}