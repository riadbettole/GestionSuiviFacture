using GestionSuiviFacture.WPF.ViewModels;
using Serilog;
using Velopack.Exceptions;
using Velopack.Sources;

namespace GestionSuiviFacture.WPF.Services.Update;

public class UpdateWindowManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WindowManager _windowManager;
    private UpdateViewModel? _updateViewModel;

    public UpdateWindowManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> CheckAndHandleUpdatesAsync(UpdateViewModel updateViewModel)
    {
        _updateViewModel = updateViewModel;

        try
        {
            _updateViewModel?.UpdateStatus("Vérification des mises à jour...");

            var github = new GithubSource(
                repoUrl: "https://github.com/riadbettole/GestionSuiviFacture",
                accessToken: null,
                prerelease: false
            );

            var mgr = new Velopack.UpdateManager(github);
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
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _updateViewModel?.SetProgress(percentage, false, $"{percentage}% terminé");
                });
            };

            await mgr.DownloadUpdatesAsync(newVersion, progressCallback);

            _updateViewModel?.UpdateStatus("Préparation de l'installation...");
            _updateViewModel?.SetProgress(100, false, "Téléchargement terminé");
            await Task.Delay(500);

            mgr.ApplyUpdatesAndRestart(newVersion);

            return true;
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
}
