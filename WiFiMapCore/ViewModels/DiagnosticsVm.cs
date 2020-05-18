using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Network;

namespace WiFiMapCore.ViewModels
{

    public class DiagnosticsNetworkVm : BaseVm, INetworkInfo
    {
        private INetworkInfo _networkInfo;

        public DiagnosticsNetworkVm(INetworkInfo networkInfo, bool isConnected)
        {
            IsConnected = isConnected;
            _networkInfo = networkInfo;
        }

        public string Ssid => _networkInfo.Ssid;

        public string Mac => _networkInfo.Mac;

        public int Rssi => _networkInfo.Rssi;

        public uint LinkQuality => _networkInfo.LinkQuality;

        public uint Channel => _networkInfo.Channel;

        public uint ChCenterFrequency => _networkInfo.ChCenterFrequency;
        
        public bool IsConnected { get; }
    }

    public class DiagnosticsVm : BaseVm
    {
        private readonly NetworksSource _networksSource = new NetworksSource();

        private readonly Timer _timer;
        private ObservableCollection<DiagnosticsNetworkVm> _details = new ObservableCollection<DiagnosticsNetworkVm>();

        public DiagnosticsVm()
        {
            _timer = new Timer(Settings.DiagnosticsUpdatePeriod.TotalMilliseconds);

            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public ObservableCollection<DiagnosticsNetworkVm> Details
        {
            get => _details;
            private set
            {
                _details = value;
                OnPropertyChanged(nameof(Details));
            }
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var bssInfo = await _networksSource.ReadNetworks().ToListAsync();
            var connected = (await _networksSource.GetConnected().ToListAsync()).ToHashSet();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Details.Clear();
                foreach (var item in bssInfo) Details.Add(new DiagnosticsNetworkVm(item, connected.Contains(item.Mac)));
            });
        }
    }
}