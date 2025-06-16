using Serilog;
using System.IO;

namespace GestionSuiviFacture.WPF.Configuration;

public static class LoggingConfig
{
    public static void Setup()
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