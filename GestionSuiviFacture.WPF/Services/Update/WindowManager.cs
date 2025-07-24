using GestionSuiviFacture.WPF.Services.Auth;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GestionSuiviFacture.WPF.Services.Update;

public class WindowManager
{
    private readonly IServiceProvider _serviceProvider;
    private UpdateProgresBar? _updateProgressWindow;
    private UpdateViewModel? _updateViewModel;
    private IAuthService _authService;


    public WindowManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _authService = _serviceProvider.GetRequiredService<IAuthService>();
        _authService.LogoutRequired += OnLogoutRequired;
    }

    private void OnLogoutRequired(object? sender, EventArgs e)
    {
        OnLogout();
    }

    public void ShowUpdateProgressWindow(Action onCheckSuccess, UpdateViewModel updateViewModel)
    {
        _updateProgressWindow = _serviceProvider.GetRequiredService<UpdateProgresBar>();
        _updateViewModel = updateViewModel;

        _updateViewModel.AssignAction(onCheckSuccess);
        _updateProgressWindow.DataContext = _updateViewModel;

        _updateProgressWindow.Show();
        _updateViewModel.UpdateStatus("Initialisation...");
    }

    public void CloseUpdateProgressWindow()
    {
        if (_updateProgressWindow != null)
        {
            _updateProgressWindow.Close();
            _updateProgressWindow = null;
            _updateViewModel = null;
        }
    }

    public void ShowLoginWindow()
    {
        var loginWindow = _serviceProvider.GetRequiredService<Login>();
        var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

        loginViewModel.AssignAction(() => OnLoginSuccess());
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();
    }

    public void OnLoginSuccess()
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        System.Windows.Application.Current.MainWindow = mainWindow;
        mainWindow.Show();

        // Close login window if it exists
        var loginWindow = System.Windows.Application.Current.Windows.OfType<Login>().FirstOrDefault();
        loginWindow?.Close();
    }

    public void OnLogout()
    {
        var loginWindow = _serviceProvider.GetRequiredService<Login>();
        var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

        _authService.Logout();

        loginViewModel.AssignAction(() => OnLoginSuccess());
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();

        // Close main window if it exists
        var mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.Close();
    }
}