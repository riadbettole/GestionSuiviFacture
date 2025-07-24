using GestionSuiviFacture.WPF.Configuration;
using GestionSuiviFacture.WPF.Services.Update;
using GestionSuiviFacture.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Velopack;

namespace GestionSuiviFacture.WPF;

public partial class App : Application
{
    private IHost _host;
    private IServiceProvider _serviceProvider;
    private WindowManager _windowManager;
    private UpdateWindowManager _updateManager;

    public App()
    {
        _host = DependencyInjectionConfig.CreateHost();
        LoggingConfig.Setup();

        // Exception handlers
        VelopackApp.Build().Run();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        _serviceProvider = _host.Services;

        var updateViewModel = _serviceProvider.GetRequiredService<UpdateViewModel>();

        _windowManager = new WindowManager(_host.Services);
        _updateManager = new UpdateWindowManager(_host.Services);

        _windowManager.ShowUpdateProgressWindow(OnCheckSuccess, updateViewModel);
        await _updateManager.CheckAndHandleUpdatesAsync(updateViewModel);

        base.OnStartup(e);
    }

    private void OnCheckSuccess()
    {
        _windowManager.ShowLoginWindow();
        _windowManager.CloseUpdateProgressWindow();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }

    // Static accessor for services (use sparingly)
    public static T GetService<T>() where T : notnull
    {
        return ((App)Current)._serviceProvider.GetRequiredService<T>();
    }
}