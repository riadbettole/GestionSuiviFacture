using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.Services.Network;
using GestionSuiviFacture.WPF.Services.Update;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly NetworkService _networkService;
    private readonly WindowManager _windowManager;
    private readonly IAuthService _authService;

    // Expose NavigationService so the View can bind to it
    public NavigationService NavigationService { get; }

    [ObservableProperty]
    private string currentUser;

    [ObservableProperty]
    private string firstLetterNameUser;

    [ObservableProperty]
    private string currentPageTitle = "Tableau de bord";

    [ObservableProperty]
    private string currentPageSubtitle = "Bienvenue dans le système de gestion";

    [ObservableProperty]
    private string connexionStatus;

    [ObservableProperty]
    private string connexionStatusColor;

    [ObservableProperty]
    private string? activePage;

    public MainViewModel(IAuthService authService, WindowManager windowManager, NetworkService networkService, NavigationService navigationService)
    {
        _authService = authService;
        NavigationService = navigationService; // Store as public property instead of private field
        _networkService = networkService;
        _windowManager = windowManager;

        // Initialize properties that depend on injected services
        CurrentUser = _authService.Username ?? string.Empty;
        FirstLetterNameUser = !string.IsNullOrEmpty(_authService.Username)
            ? _authService.Username.Substring(0, 1).ToUpper()
            : "U";

        // Set up network monitoring
        _networkService.NetworkStatusChanged += OnNetworkStatusChanged;
        _networkService.StartMonitoring();

        // Initialize connection status
        ConnexionStatus = "Connecté";
        ConnexionStatusColor = "Green";
    }

    [RelayCommand]
    private void Disconnect()
    {
        _windowManager.OnLogout();
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

        // Create FactureViewModel through DI container
        var factureViewModel = App.GetService<FactureViewModel>();
        NavigationService.NavigateTo(factureViewModel);
    }

    [RelayCommand]
    public void NavigateToConsultation()
    {
        CurrentPageTitle = "Consultations";
        CurrentPageSubtitle = "Recherche et consultation d'étiquettes";
        SetActivePage("Consultation");

        // Create ConsultationViewModel through DI container
        var consultationViewModel = App.GetService<ConsultationViewModel>();
        NavigationService.NavigateTo(consultationViewModel);
    }

    public void SetActivePage(string page)
    {
        ActivePage = page;
    }
}