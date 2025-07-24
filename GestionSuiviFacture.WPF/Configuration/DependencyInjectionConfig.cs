using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.Services.Auth;
using GestionSuiviFacture.WPF.Services.Network;
using GestionSuiviFacture.WPF.Services.Saisie;
using GestionSuiviFacture.WPF.Services.Update;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GestionSuiviFacture.WPF.Configuration;

public static class DependencyInjectionConfig
{
    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddSingleton<WindowManager>();
        services.AddSingleton<UpdateWindowManager>();

        // Business Services
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ICommandeService, CommandeService>();
        services.AddSingleton<IFactureService, FactureService>();
        services.AddSingleton<IEtiquetteService, EtiquetteService>();

        // Application Services
        services.AddSingleton<NavigationService>();
        services.AddSingleton<NetworkService>();
        services.AddSingleton<StorageCredential>();
        services.AddSingleton<IPrintService, PrintService>();
        services.AddSingleton<IAuthenticatedHttpClient, AuthenticatedHttpClient>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<UpdateViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<FactureViewModel>();
        services.AddTransient<ConsultationViewModel>();

        // Views
        services.AddTransient<Login>();
        services.AddTransient<MainWindow>();
        services.AddSingleton<UpdateProgresBar>();
    }
}