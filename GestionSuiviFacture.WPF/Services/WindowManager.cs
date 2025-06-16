using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GestionSuiviFacture.WPF.Services.Utilities;

public class WindowManager
{
    private readonly IServiceProvider _serviceProvider;
    private UpdateProgresBar? _updateProgressWindow;
    private UpdateViewModel? _updateViewModel;

    public WindowManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        App.Current.MainWindow = mainWindow;
        mainWindow.Show();

        // Close login window if it exists
        var loginWindow = App.Current.Windows.OfType<Login>().FirstOrDefault();
        loginWindow?.Close();
    }

    public void OnLogout()
    {
        var loginWindow = _serviceProvider.GetRequiredService<Login>();
        var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

        loginViewModel.AssignAction(() => OnLoginSuccess());
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();

        // Close main window if it exists
        var mainWindow = App.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.Close();
    }
}