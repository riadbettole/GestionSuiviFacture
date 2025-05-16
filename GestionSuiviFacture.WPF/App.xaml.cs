using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;

namespace GestionSuiviFacture.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {

        base.OnStartup(e);

        LoginViewModel lvm = new LoginViewModel();
        lvm.AssignAction(OnLoginSuccess);

        var loginWindow = new Login
        {
            DataContext = lvm
        };

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
}

