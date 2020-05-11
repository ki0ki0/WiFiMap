using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Timers;
using System.Windows;
using WiFiMapCore.Interfaces.Network;
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
            var diagnosticsUpdatePeriod = ConfigurationManager.AppSettings.Get("DiagnosticsUpdatePeriod");
            TimeSpan period;
            if (TimeSpan.TryParse(diagnosticsUpdatePeriod, out period))
            {
                period = TimeSpan.FromSeconds(1);
            }
            _timer = new Timer(period.TotalMilliseconds);
            
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