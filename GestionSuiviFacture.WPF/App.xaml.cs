using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;
using Velopack;

namespace GestionSuiviFacture.WPF;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        VelopackApp.Build().Run();

        await UpdateMyApp();

        var loginWindow = new Login();
        var loginViewModel = new LoginViewModel();

        loginViewModel.AssignAction(OnLoginSuccess);
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();
    }

    private void OnLoginSuccess()
    {
        var mainWindow = new MainWindow();
        Application.Current.MainWindow = mainWindow;
        mainWindow.Show();

        if (Current.Windows.Count > 0 && Current.Windows[0] is Login login)
        {
            login.Close();
        }
    }

    private static async Task UpdateMyApp()
    {
        var mgr = new UpdateManager("https://github.com/riadbettole/GestionSuiviFacture/releases");

        // check for new version
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
            return; // no update available

        // download new version
        await mgr.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        mgr.ApplyUpdatesAndRestart(newVersion);
    }

    public void OnLogout()
    {
        // Show the login window again
        var loginWindow = new Login();
        var loginViewModel = new LoginViewModel();

        loginViewModel.AssignAction(OnLoginSuccess);
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();

        // Close the main window
        if (Current.Windows.Count > 0 && Current.Windows[0] is MainWindow mainWindow)
        {
            mainWindow.Close();
        }
    }

}

