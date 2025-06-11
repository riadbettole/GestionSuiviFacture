using GestionSuiviFacture.WPF;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Timers;

namespace GestionSuiviFacture.WPF.Services.Utilities
{
    public class NetworkService : IDisposable
    {
        private readonly System.Timers.Timer _networkCheckTimer;
        private DateTime _lastConnectedTime;
        private bool _isConnected = true;

        public event EventHandler<bool>? NetworkStatusChanged;

        public bool IsConnected => _isConnected;

        public NetworkService(int checkIntervalMs = 5000)
        {
            _lastConnectedTime = DateTime.Now;

            _networkCheckTimer = new System.Timers.Timer(checkIntervalMs);
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

        private async Task<bool> IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync("8.8.8.8", 3000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> IsLocalServerAvailable()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);

                var response = await client.GetAsync("https://localhost:7167");

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
            //bool isConnected = await IsInternetAvailable();

            App.Current?.Dispatcher.Invoke(() =>
            {
                if (isConnected)
                {
                    _lastConnectedTime = DateTime.Now;
                    if (!_isConnected)
                    {
                        _isConnected = true;
                        NetworkStatusChanged?.Invoke(this, true);
                    }
                }
                else
                {
                    if ((DateTime.Now - _lastConnectedTime).TotalSeconds > 5)
                    {
                        if (_isConnected)
                        {
                            _isConnected = false;
                            NetworkStatusChanged?.Invoke(this, false);
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            _networkCheckTimer?.Stop();
            _networkCheckTimer?.Dispose();
        }
    }
}