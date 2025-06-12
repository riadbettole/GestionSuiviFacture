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

        // Start progress window immediately
        StartProgressBar();

        // Then start the update check process
        await StartUpdateProcess();

        //StartLoginWindow();

        base.OnStartup(e);
    }

    private void StartProgressBar()
    {
        _updateProgressWindow = new UpdateProgresBar();
        _updateViewModel = new UpdateViewModel();

        _updateViewModel.AssignAction(OnCheckSuccess);
        _updateProgressWindow.DataContext = _updateViewModel;

        _updateProgressWindow.Show();

        // Set initial status
        _updateViewModel.UpdateStatus("Initialisation...");
    }

    private async Task StartUpdateProcess()
    {
        // Add a small delay to ensure the window is visible
        await Task.Delay(500);

        // Check and handle updates - this will trigger the event when done
        await CheckAndHandleUpdates();
    }

    private async Task<bool> CheckAndHandleUpdates()
    {
        try
        {
            Log.Information("Starting update check...");
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

            Log.Information($"Update found: {newVersion.TargetFullRelease.Version}");
            _updateViewModel?.UpdateStatus($"Mise à jour trouvée: {newVersion.TargetFullRelease.Version}");

            // Download with progress using Action<int>
            Log.Information("Downloading updates...");
            _updateViewModel?.UpdateStatus("Téléchargement des mises à jour...");
            _updateViewModel?.SetProgress(0, false, "Préparation du téléchargement...");

            // Create progress callback as Action<int>
            Action<int> progressCallback = percentage =>
            {
                // Use Dispatcher to update UI from background thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _updateViewModel?.SetProgress(percentage, false, $"{percentage}% terminé");
                });
            };

            await mgr.DownloadUpdatesAsync(newVersion, progressCallback);

            Log.Information("Preparing installation...");
            _updateViewModel?.UpdateStatus("Préparation de l'installation...");
            _updateViewModel?.SetProgress(100, false, "Téléchargement terminé");
            await Task.Delay(500);

            CloseProgressWindow();
            Log.Information("Starting installation - Velopack will take over...");
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

    private void StartLoginWindow()
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

    private void SetupLogging()
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