using System.IO;
using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;
using Serilog;
using Velopack;
using Velopack.Exceptions;
using Velopack.Sources;

namespace GestionSuiviFacture.WPF;

public partial class App : Application
{
    [STAThread]
    private static void Main(string[] args) 
    {
        SetupLogging();
        Log.Information("App starting (Main)...");
        try
        {
            VelopackApp.Build().Run();

            _ = Task.Run(async () =>
            {
                try
                {
                    await UpdateMyApp();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Update failed");
                }
            });

            var app = new App();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            app.InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
                Log.Fatal(ex.ExceptionObject as Exception, "Unhandled domain exception");
            app.DispatcherUnhandledException += (s, ex) =>
            {
                Log.Error(ex.Exception, "Unhandled UI exception");
                ex.Handled = true;
            };

            var loginWindow = new Login();
            var loginViewModel = new LoginViewModel();
            loginViewModel.AssignAction(OnLoginSuccess);
            loginWindow.DataContext = loginViewModel;
            loginWindow.Show();

            app.Run();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Fatal error: " + ex.ToString());
            Log.Fatal(ex, "Fatal error in Main()");
        }
    }

    private static void SetupLogging()
    {
        // Create logs folder in app data
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GestionSuiviFacture",
            "logs",
            "app-.txt"
        );

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    private static async Task UpdateMyApp()
    {
        try
        {
            Log.Information("Starting update check...");

            var github = new GithubSource(
                repoUrl: "https://github.com/riadbettole/GestionSuiviFacture",
                accessToken: null,
                prerelease: false
            );

            var mgr = new UpdateManager(github);

            Log.Information("Checking for updates...");
            var newVersion = await mgr.CheckForUpdatesAsync();

            if (newVersion == null)
            {
                Log.Information("No updates available");
                return;
            }

            Log.Information($"Update found: {newVersion.TargetFullRelease.Version}");
            Log.Information($"Current version: {newVersion.BaseRelease?.Version}");

            // For testing, just log what would happen instead of actually updating

            Log.Information($"Update found: {newVersion.TargetFullRelease.Version}");

            Log.Information("Downloading updates...");
            await mgr.DownloadUpdatesAsync(newVersion);

            Log.Information("Applying updates and restarting...");
            mgr.ApplyUpdatesAndRestart(newVersion);
        }
        catch (NotInstalledException ex)
        {
            Log.Information(ex, "App not installed via Velopack (probably debug mode)");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Update check failed");
        }
    }

    private static void OnLoginSuccess()
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

