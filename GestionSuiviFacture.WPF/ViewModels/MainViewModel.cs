using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
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
    private static void Disconnect()
    {
        App.OnLogout();
    }

    public NavigationService Navigation { get; }
    private readonly NetworkService _networkService;

    public MainViewModel()
    {
        Navigation = new NavigationService();

        _networkService = new NetworkService(3000); // Check every 3 seconde
        _networkService.NetworkStatusChanged += OnNetworkStatusChanged;

        ConnexionStatus = "Connecté";
        ConnexionStatusColor = "Green";

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
    private string? activePage;

    public void SetActivePage(string page)
    {
        ActivePage = page;
    }
}
