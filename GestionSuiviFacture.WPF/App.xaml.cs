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
    private UpdateProgresBar? _updateProgressWindow;
    private UpdateViewModel? _updateViewModel;

    protected override async void OnStartup(StartupEventArgs e)
    {
        SetupLogging();
        Log.Information("Application starting...");

        AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            Log.Fatal(ex.ExceptionObject as Exception, "Unhandled exception occurred");

        DispatcherUnhandledException += (s, ex) =>
        {
            Log.Error(ex.Exception, "Unhandled UI exception occurred");
            ex.Handled = true;
        };

        VelopackApp.Build().Run();

        StartProgressBar();

        await StartUpdateProcess();

        base.OnStartup(e);
    }

    private void StartProgressBar()
    {
        _updateProgressWindow = new UpdateProgresBar();
        _updateViewModel = new UpdateViewModel();

        _updateViewModel.AssignAction(OnCheckSuccess);
        _updateProgressWindow.DataContext = _updateViewModel;

        _updateProgressWindow.Show();

        _updateViewModel.UpdateStatus("Initialisation...");
    }

    private async Task StartUpdateProcess()
    {
        await Task.Delay(500);

        await CheckAndHandleUpdates();
    }

    private async Task<bool> CheckAndHandleUpdates()
    {
        try
        {
            _updateViewModel?.UpdateStatus("Vérification des mises à jour...");

            var github = new GithubSource(
                repoUrl: "https://github.com/riadbettole/GestionSuiviFacture",
                accessToken: null,
                prerelease: false
            );

            var mgr = new UpdateManager(github);
            var newVersion = await mgr.CheckForUpdatesAsync();

            if (newVersion == null)
            {
                Log.Information("No updates available");
                _updateViewModel?.UpdateStatus("Aucune mise à jour disponible");
                await Task.Delay(800);

                _updateViewModel?.CompleteUpdateCheck();
                return false;
            }

            Log.Information("Update found: {Version}", newVersion.TargetFullRelease.Version);
            _updateViewModel?.UpdateStatus(
                $"Mise à jour trouvée: {newVersion.TargetFullRelease.Version}"
            );

            _updateViewModel?.UpdateStatus("Téléchargement des mises à jour...");
            _updateViewModel?.SetProgress(0, false, "Préparation du téléchargement...");

            Action<int> progressCallback = percentage =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    _updateViewModel?.SetProgress(percentage, false, $"{percentage}% terminé");
                });
            };

            await mgr.DownloadUpdatesAsync(newVersion, progressCallback);

            _updateViewModel?.UpdateStatus("Préparation de l'installation...");
            _updateViewModel?.SetProgress(100, false, "Téléchargement terminé");
            await Task.Delay(500);

            CloseProgressWindow();
            mgr.ApplyUpdatesAndRestart(newVersion);

            return false;
        }
        catch (NotInstalledException ex)
        {
            Log.Information(ex, "App not installed via Velopack (probably debug mode)");
            _updateViewModel?.UpdateStatus("Mode développement détecté");
            await Task.Delay(800);

            _updateViewModel?.CompleteUpdateCheck();
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Update check failed");
            _updateViewModel?.UpdateStatus($"Erreur lors de la vérification: {ex.Message}");
            await Task.Delay(1200);

            _updateViewModel?.CompleteUpdateCheck();
            return false;
        }
    }

    private void CloseProgressWindow()
    {
        if (_updateProgressWindow != null)
        {
            _updateProgressWindow.Close();
            _updateProgressWindow = null;
            _updateViewModel = null;
        }
    }

    private static void StartLoginWindow()
    {
        var loginWindow = new Login();
        var loginViewModel = new LoginViewModel();

        loginViewModel.AssignAction(OnLoginSuccess);
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();
    }

    private void OnCheckSuccess()
    {
        StartLoginWindow();
        CloseProgressWindow();
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

    public static void OnLogout()
    {
        var loginWindow = new Login();
        var loginViewModel = new LoginViewModel();

        loginViewModel.AssignAction(OnLoginSuccess);
        loginWindow.DataContext = loginViewModel;

        loginWindow.Show();

        if (Current.Windows.Count > 0 && Current.Windows[0] is MainWindow mainWindow)
        {
            mainWindow.Close();
        }
    }

    private static void SetupLogging()
    {
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
}
