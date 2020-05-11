using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Timers;
using System.Windows;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Network;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace WiFiMapCore.ViewModels
{
    public class DiagnosticsVm : BaseVm
    {
        private ObservableCollection<INetworkInfo> _details = new ObservableCollection<INetworkInfo>();

        private readonly Timer _timer;
        public ObservableCollection<INetworkInfo> Details
        {
            get => _details;
            private set
            {
                _details = value;
                OnPropertyChanged(nameof(Details));
            }
        }
        
        private readonly NetworksSource _networksSource = new NetworksSource();

        public DiagnosticsVm()
        {
            _timer = new Timer(Settings.DiagnosticsUpdatePeriod.TotalMilliseconds);
            
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var bssInfo = await _networksSource.ReadNetworks().ToListAsync();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Details.Clear();
                foreach (var item in bssInfo)
                {
                    Details.Add(item);
                };
            });
            
        }
    }
}