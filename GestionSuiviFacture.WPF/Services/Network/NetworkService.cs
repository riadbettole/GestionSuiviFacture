using System.Net.Http;
using System.Timers;

namespace GestionSuiviFacture.WPF.Services.Network;

public class NetworkService
{
    private readonly System.Timers.Timer _networkCheckTimer;
    private DateTime _lastConnectedTime;
    private bool _isConnected = true;

    public event EventHandler<bool>? NetworkStatusChanged;

    public bool IsConnected => _isConnected;

    public NetworkService()
    {
        _lastConnectedTime = DateTime.UtcNow;

        _networkCheckTimer = new System.Timers.Timer(3000);
        _networkCheckTimer.Elapsed += CheckNetworkStatus;
    }

    public void StartMonitoring()
    {
        _networkCheckTimer.Start();
    }

    public void StopMonitoring()
    {
        _networkCheckTimer.Stop();
    }

    private static async Task<bool> IsLocalServerAvailable()
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(3);

            await client.GetAsync("https://localhost:7167/api/v1/network/health");

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async void CheckNetworkStatus(object? sender, ElapsedEventArgs e)
    {
        bool isConnected = await IsLocalServerAvailable();

        if (System.Windows.Application.Current?.Dispatcher != null)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (isConnected)
                {
                    _lastConnectedTime = DateTime.UtcNow;
                    if (!_isConnected)
                    {
                        _isConnected = true;
                        NetworkStatusChanged?.Invoke(this, true);
                    }
                }
                else
                {
                    if ((DateTime.UtcNow - _lastConnectedTime).TotalSeconds > 5 && _isConnected)
                    {
                        _isConnected = false;
                        NetworkStatusChanged?.Invoke(this, false);
                    }
                }
            });
        }
    }
}
