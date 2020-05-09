using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WiFiMapCore.HeatMap;
using WiFiMapCore.Interfaces.Project;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Network;
using WiFiMapCore.Model.Project;

namespace WiFiMapCore.ViewModels
{
    public class ProjectVm : BaseVm
    {
        private ImageSource _image = new BitmapImage();
        private bool _isModified;
        private ObservableCollection<IScanPoint> _items = new ObservableCollection<IScanPoint>();
        private ObservableCollection<NetworkVm> _networks = new ObservableCollection<NetworkVm>();
        private readonly NetworksSource _networksSource = new NetworksSource();
        private ObservableCollection<HeatPoint> _points = new ObservableCollection<HeatPoint>();
        private Project _project = new Project();
        private double _scaleFactor = 1;
        private double _scaleFactorMax = 10;
        private double _scaleFactorMin = 0.1;

        private TaskCompletionSource<IProject> _taskCompletionSource = new TaskCompletionSource<IProject>();

        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                Image = ImageCoder.ByteToImage(Project.Bitmap);
                Items = new ObservableCollection<IScanPoint>(Project.ScanPoints);
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

        public ICommand Click => new Command<MouseButtonEventArgs>(OnClick);

        public ObservableCollection<IScanPoint> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
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

        public ICommand Wheel => new Command<MouseWheelEventArgs>(OnWheel);

        public ObservableCollection<HeatPoint> Points
        {
            get => _points;
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
            }
        }

        private async void OnClick(MouseButtonEventArgs e)
        {
            var inputElement = e.Source as IInputElement;
            var position = e.GetPosition(inputElement);
            var bssInfo = await _networksSource.ReadNetworks().ToListAsync();
            var scanPoint = new ScanPoint((int) position.X, (int) position.Y, bssInfo);
            Items.Add(scanPoint);
            Project.ScanPoints.Add(scanPoint);
            IsModified = true;
            Update();
        }

        private void OnWheel(MouseWheelEventArgs obj)
        {
            if (Keyboard.PrimaryDevice.IsKeyDown(Key.LeftCtrl) || Keyboard.PrimaryDevice.IsKeyDown(Key.RightCtrl))
            {
                ScaleFactor += (double) obj.Delta / 100;
                obj.Handled = true;
            }
        }

        private void Update()
        {
            var groupBy = Items.SelectMany(point => point.BssInfo).GroupBy(info => info.Ssid);

            var networkVms = groupBy.Select(gr =>
                {
                    var macs = gr.Select(entity => entity.Mac).Distinct();
                    return new NetworkVm(gr.Key, macs);
                })
                .ToArray();

            foreach (var networkVm in networkVms) networkVm.PropertyChanged += NetworkVmOnPropertyChanged;

            Networks = new ObservableCollection<NetworkVm>(networkVms);

            UpdateHeatMap();
        }

        private void NetworkVmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateHeatMap();
        }

        private void UpdateHeatMap()
        {
            var heatPoints = new List<HeatPoint>();

            var macs = Networks.SelectMany(vm =>
            {
                return vm.Children
                    .Where(networkVm => networkVm.IsChecked != false)
                    .Select(i => i.Name);
            }).ToHashSet();
            
            foreach (var scanPoint in Items)
            {
                var entities = scanPoint.BssInfo.Where(info => macs.Contains(info.Mac)).ToArray();
                if (entities.Length > 0)
                {
                    var max = entities.Max(i => i.Rssi);
                    max = max + 80;
                    if (max < 0)
                    {
                        max = 0;
                    }
                    max = max * max / 4;
                    if (max > 255)
                    {
                        max = 255;
                    }
                    heatPoints.Add(new HeatPoint(scanPoint.Position.X, scanPoint.Position.Y, (byte) (max)));
                }
            }

            Points = new ObservableCollection<HeatPoint>(heatPoints);
        }
    }
}