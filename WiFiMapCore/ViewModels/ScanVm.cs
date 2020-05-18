using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WiFiMapCore.HeatMap;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Interfaces.Project;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Network;
using WiFiMapCore.Model.Project;

namespace WiFiMapCore.ViewModels
{
    public class ScanVm : BaseVm
    {
        private readonly NetworksSource _networksSource = new NetworksSource();
        private IScanPoint? _currentDetailsPoint;
        private ObservableCollection<INetworkInfo> _details = new ObservableCollection<INetworkInfo>();
        private ObservableCollection<IHeatPoint> _heatPoints = new ObservableCollection<IHeatPoint>();
        private ImageSource _image = new BitmapImage();
        private bool _isModified;
        private ObservableCollection<NetworkVm> _networks = new ObservableCollection<NetworkVm>();
        private ProgressControlVm? _progressVm;
        private Project _project = new Project();
        private double _scaleFactor = 1;
        private double _scaleFactorMax = 10;
        private double _scaleFactorMin = 0.1;
        private ObservableCollection<IScanPoint> _scanPoints = new ObservableCollection<IScanPoint>();
        private DateTime? _lastClick;

        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                Image = ImageCoder.ByteToImage(Project.Bitmap);
                Update();
            }
        }

        public ImageSource Image
        {
            get => _image;
            private set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public ObservableCollection<IHeatPoint> HeatPoints
        {
            get => _heatPoints;
            set
            {
                _heatPoints = value;
                OnPropertyChanged(nameof(HeatPoints));
            }
        }

        public ObservableCollection<NetworkVm> Networks
        {
            get => _networks;
            private set
            {
                _networks = value;
                OnPropertyChanged(nameof(Networks));
            }
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

        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value < ScaleFactorMin || value > ScaleFactorMax) return;

                _scaleFactor = value;
                OnPropertyChanged(nameof(ScaleFactor));
            }
        }

        public double ScaleFactorMin
        {
            get => _scaleFactorMin;
            set
            {
                _scaleFactorMin = value;
                OnPropertyChanged(nameof(ScaleFactorMin));
            }
        }

        public double ScaleFactorMax
        {
            get => _scaleFactorMax;
            set
            {
                _scaleFactorMax = value;
                OnPropertyChanged(nameof(ScaleFactorMax));
            }
        }

        public ICommand Click => new AsyncCommand<MouseButtonEventArgs>(OnClick);

        public ICommand ScanPointHover => new Command<MouseEventArgs>(OnHover);
        public ICommand ScanPointClick => new Command<MouseButtonEventArgs>(OnScanPointClick);

        public ICommand Wheel => new Command<MouseWheelEventArgs>(OnWheel);

        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public ProgressControlVm? ProgressVm
        {
            get => _progressVm;
            set
            {
                _progressVm = value;
                OnPropertyChanged(nameof(ProgressVm));
            }
        }

        private async Task OnClick(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (DateTime.Now - _lastClick < TimeSpan.FromSeconds(0.5))
                {
                    using (ProgressVm = new ProgressControlVm())
                    {
                        var inputElement = e.Source as IInputElement;
                        var position = e.GetPosition(inputElement);
                        await _networksSource.ForceUpdate(ProgressVm.Token);

                        var minInfos = await _networksSource.ReadNetworks(ProgressVm.Token)
                            .ToListAsync(ProgressVm.Token);

                        var scanPoint = new ScanPoint((int) position.X, (int) position.Y, minInfos);
                        Project.ScanPoints.Add(scanPoint);
                        IsModified = true;
                        Update();
                    }
                }

                _lastClick = DateTime.Now;
            }
        }


        private void OnWheel(MouseWheelEventArgs obj)
        {
            if (Keyboard.PrimaryDevice.IsKeyDown(Key.LeftCtrl) || Keyboard.PrimaryDevice.IsKeyDown(Key.RightCtrl))
            {
                ScaleFactor += (double) obj.Delta / 100;
                obj.Handled = true;
            }
        }

        private void OnHover(MouseEventArgs e)
        {
            var inputElement = e.Source as FrameworkElement;
            _currentDetailsPoint =  inputElement?.DataContext as IScanPoint;
            UpdateDetails();
        }
        
        private void OnScanPointClick(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var inputElement = e.Source as FrameworkElement;

                if (inputElement?.DataContext is IScanPoint point)
                {
                    var single = Project.ScanPoints.Single( scanPoint => scanPoint.Position == point.Position);
                    Project.ScanPoints.Remove(single);
                    
                    IsModified = true;
                    Update();
                }
            }
        }

        private async void UpdateDetails()
        {
            if (_currentDetailsPoint == null)
                return;

            var macs = Networks.SelectMany(vm =>
            {
                return vm.Children
                    .Where(networkVm => networkVm.IsChecked != false)
                    .Select(i => i.Mac);
            }).ToHashSet();

            Details.Clear();
            
            var connected = (await _networksSource.GetConnected().ToListAsync()).ToHashSet();

            var orderedEnumerable = _currentDetailsPoint.BssInfo
                .OrderBy(i => i.Ssid)
                .ThenBy(i => i.Channel)
                .ThenBy(i => i.Mac);
            foreach (var networkInfo in orderedEnumerable)
            {
                var contains = macs.Contains(networkInfo.Mac);
                Details.Add(new NetworkInfoDetailsVm(networkInfo, contains, connected.Contains(networkInfo.Mac)));
            }
        }

        private async void Update()
        {
            var checkedItems = Networks.SelectMany(vm => vm.Children).Where(vm => vm.IsChecked == true).ToLookup(vm => vm.Mac);
            var expandedItems = Networks.Where(vm => vm.IsExpanded).ToLookup(vm => vm.Name);

            var groupBy = Project.ScanPoints.SelectMany(point => point.BssInfo).GroupBy(info => info.Ssid);
            
            var connected = (await _networksSource.GetConnected().ToListAsync()).ToHashSet();

            var networkVms = groupBy.Select(gr =>
                {
                    var macEqualityComparer = new MacEqualityComparer();
                    var macs = gr.Distinct(macEqualityComparer);
                    var vms = macs
                        .OrderBy(info => info.Channel)
                        .ThenBy(info => info.Mac)
                        .Select(mac => new NetworkVm(mac, connected.Contains(mac.Mac)));
                    return new NetworkVm(gr.Key, vms);
                })
                .ToArray();

            foreach (var networkVm in networkVms) networkVm.PropertyChanged += NetworkVmOnPropertyChanged;

            var toCheck = networkVms.SelectMany(vm => vm.Children).Where(vm => checkedItems.Contains(vm.Mac));
            foreach (var vm in toCheck)
            {
                vm.IsChecked = true;
            }
            
            var toExpand = networkVms.Where(vm => expandedItems.Contains(vm.Name));
            foreach (var vm in toExpand)
            {
                vm.IsExpanded = true;
            }

            Networks = new ObservableCollection<NetworkVm>(networkVms.OrderBy(vm => vm.Name));

            UpdateHeatMap();
        }

        private void NetworkVmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateHeatMap();
            UpdateDetails();
        }

        private void UpdateHeatMap()
        {
            var heatPoints = new List<HeatPointVm>();

            var macs = Networks.SelectMany(vm =>
            {
                return vm.Children
                    .Where(networkVm => networkVm.IsChecked != false)
                    .Select(i => i.Mac);
            }).ToHashSet();

            foreach (var scanPoint in Project.ScanPoints)
            {
                var entities = scanPoint.BssInfo.Where(info => macs.Contains(info.Mac)).ToArray();
                if (entities.Length > 0)
                {
                    var rssi = entities.Max(i => i.Rssi);
                     rssi = rssi + 80;
                     if (rssi < 0) rssi = 0;
                    
                    rssi = (int)(rssi*6.4);
                    if (rssi > 255) rssi = 255;
                    heatPoints.Add(new HeatPointVm(scanPoint.Position, (byte) rssi, scanPoint.BssInfo));
                }
                else
                {
                    heatPoints.Add(new HeatPointVm(scanPoint.Position, scanPoint.BssInfo));
                }
            }

            HeatPoints = new ObservableCollection<IHeatPoint>(heatPoints);
        }
    }
}