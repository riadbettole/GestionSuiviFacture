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
        [ObservableProperty] private bool _hasError = false;

        public event Action LoginSucceeded;

        public void AssignAction (Action action)
        {
            LoginSucceeded += action;
        }

        public LoginViewModel()
        {
            _authService = new AuthService();
        }

        [RelayCommand]
        private async Task Login()
        {
            HasError = false;
            ErrorMessage = string.Empty;

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

            bool isAuthenticated = await _authService.LoginAsync(Username, Password);

            if (isAuthenticated)
            {
                LoginSucceeded?.Invoke(); // Notify the window to close
            }
            else
            {
                ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect";
                HasError = true;
            }
        }
    }
}
