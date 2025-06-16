using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;
using System.Net.Http;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLogging;

    [ObservableProperty]
    private bool _rememberMe;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _loginButtonText = "Se connecter";

    public event Action? LoginSucceeded;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    public void AssignAction(Action action)
    {
        LoginSucceeded += action;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        IsLogging = true;
        LoginButtonText = "Connexion...";
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ShowError("Le nom d'utilisateur est requis");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ShowError("Le mot de passe est requis");
                return;
            }

            bool isAuthenticated = await _authService.LoginAsync(Username, Password);

            if (isAuthenticated)
            {
                HasError = false;
                ErrorMessage = string.Empty;
                LoginSucceeded?.Invoke();
            }
            else
            {
                ShowError("Nom d'utilisateur ou mot de passe incorrect");
            }
        }
        catch (InvalidOperationException ex)
        {
            ShowError(ex.Message); // Shows archived message
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") && ex.Data["StatusCode"].Equals(423))
        {
            throw new InvalidOperationException("Votre compte a été archivé. Contactez le support.");
        }
        catch (System.Net.NetworkInformation.NetworkInformationException)
        {
            ShowError("Problème de connexion réseau. Vérifiez votre connexion internet.");
        }
        catch (System.Net.Http.HttpRequestException)
        {
            ShowError("Impossible de se connecter au serveur. Réessayez plus tard.");
        }
        catch (TimeoutException)
        {
            ShowError("La connexion a expiré. Réessayez.");
        }
        catch (UnauthorizedAccessException)
        {
            ShowError("Accès non autorisé. Vérifiez vos identifiants.");
        }
        catch (Exception ex)
        {
            ShowError($"Une erreur inattendue s'est produite: {ex.Message}");
        }
        finally
        {
            IsLogging = false;
            LoginButtonText = "Se connecter";
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private bool CanLogin() =>
        !IsLogging && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrEmpty(Password);

    partial void OnUsernameChanged(string value)
    {
        if (HasError && !string.IsNullOrWhiteSpace(value))
        {
            HasError = false;
            ErrorMessage = string.Empty;
        }
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string value)
    {
        if (HasError && !string.IsNullOrEmpty(value))
        {
            HasError = false;
            ErrorMessage = string.Empty;
        }
        LoginCommand.NotifyCanExecuteChanged();
    }
}