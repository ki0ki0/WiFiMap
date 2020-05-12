using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Network;

namespace WiFiMapCore.ViewModels
{
    public class DiagnosticsVm : BaseVm
    {
        private readonly NetworksSource _networksSource = new NetworksSource();

        private readonly Timer _timer;
        private ObservableCollection<INetworkInfo> _details = new ObservableCollection<INetworkInfo>();

        public DiagnosticsVm()
        {
            _timer = new Timer(Settings.DiagnosticsUpdatePeriod.TotalMilliseconds);

            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public ObservableCollection<INetworkInfo> Details
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
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Details.Clear();
                foreach (var item in bssInfo) Details.Add(item);
                ;
            });
        }
    }
}