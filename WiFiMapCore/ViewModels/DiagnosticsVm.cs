using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        private ObservableCollection<NetworkInfoWithHistoryVm> _details = new ObservableCollection<NetworkInfoWithHistoryVm>();
        private Dictionary<string,NetworkInfoWithHistoryVm> _detailsDict = new Dictionary<string, NetworkInfoWithHistoryVm>();
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);

        public DiagnosticsVm()
        {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            UpdateLoop();
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateList();
        }

        public ObservableCollection<NetworkInfoWithHistoryVm> Details
        {
            get => _details;
            private set
            {
                _details = value;
                OnPropertyChanged(nameof(Details));
            }
        }

        private async void UpdateLoop()
        {
            while (true)
            {
                await _networksSource.ForceUpdate();
            }
        }

        private async Task UpdateList()
        {
            var bssInfo = await _networksSource.ReadNetworks().ToListAsync();
            var connected = (await _networksSource.GetConnected().ToListAsync()).ToHashSet();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var updated = new HashSet<NetworkInfoWithHistoryVm>();
                foreach (var networkInfo in bssInfo)
                {
                    NetworkInfoWithHistoryVm? vm;
                    if (_detailsDict.TryGetValue(networkInfo.Mac, out vm))
                    {
                        vm.AddRssi(networkInfo.Rssi, networkInfo.LinkQuality);
                    }
                    else
                    {
                        vm = new NetworkInfoWithHistoryVm(networkInfo, connected.Contains(networkInfo.Mac));
                        _detailsDict.Add(networkInfo.Mac, vm);
                        Details.Add(vm);
                    }

                    updated.Add(vm);
                }

                var missing = _detailsDict.Values.Where(i => !updated.Contains(i));
                foreach (var vm in missing)
                {
                    vm.AddRssi(vm.Rssi, vm.LinkQuality);
                }
            });
        }
    }
}