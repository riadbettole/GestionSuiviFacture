using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        //private readonly IAuthService _authService;
        private readonly AuthService _authService;

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _errorMessage = string.Empty;
        [ObservableProperty] private bool _isLogging;
        [ObservableProperty] private bool _rememberMe;
        [ObservableProperty] private bool _hasError;
        [ObservableProperty] private string _loginButtonText = "Se connecter";

        public event Action? LoginSucceeded;

        public void AssignAction(Action action)
        {
            LoginSucceeded += action;
        }

        public LoginViewModel()
        {
            _authService = new AuthService();
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
                // Input validation
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

                // Simulate network delay
                //await Task.Delay(1000);

                bool isAuthenticated = await _authService.LoginAsync(Username, Password);

                if (isAuthenticated)
                {
                    // Clear any previous errors
                    HasError = false;
                    ErrorMessage = string.Empty;

                    // Notify success
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    ShowError("Nom d'utilisateur ou mot de passe incorrect");
                }
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

        private bool CanLogin() => !IsLogging &&
                                  !string.IsNullOrWhiteSpace(Username) &&
                                  !string.IsNullOrEmpty(Password);

        // Method to clear errors when user starts typing
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
}
