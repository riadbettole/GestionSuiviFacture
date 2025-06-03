using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;

namespace GestionSuiviFacture.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {

        base.OnStartup(e);

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

