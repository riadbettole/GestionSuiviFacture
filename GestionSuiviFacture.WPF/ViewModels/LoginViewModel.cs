using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly Action _onLoginSuccess;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public LoginViewModel()
        {
            // Default constructor if needed for design-time support
        }

        public LoginViewModel(Action onLoginSuccess = null)
        {
            _onLoginSuccess = onLoginSuccess;
        }

        [RelayCommand]
        private async Task Login()
        {
            // Reset error state
            HasError = false;
            ErrorMessage = string.Empty;

            // Validate input
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Le nom d'utilisateur est requis";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Le mot de passe est requis";
                HasError = true;
                return;
            }

            var authService = new AuthService();
            bool isAuthenticated = await authService.LoginAsync(Username, Password);

            //bool isAuthenticated = AuthenticateUser();


            if (isAuthenticated)
            {
                _onLoginSuccess?.Invoke();

                
                if (_onLoginSuccess == null)
                {
                    if (Application.Current.MainWindow is Window mainWindow)
                    {
                        var mainViewModel = new MainViewModel();
                        mainWindow.DataContext = mainViewModel;
                        mainViewModel.NavigateToFacture();
                    }
                }
            }
            else
            {
                ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect";
                HasError = true;
            }
        }

        private bool AuthenticateUser()
        {
            return (Username == "admin" && Password == "admin");
        }

        public void Dispose()
        { }
    }
}
