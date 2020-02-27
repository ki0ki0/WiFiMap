﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WiFIMap.HeatMap;
using WiFIMap.Interfaces;
using WiFIMap.Model;
using WiFIMap.Model.Network;

namespace WiFIMap.ViewModels
{
    public class ResultVm : BaseVm, IProjectContainerVm
    {
        private ImageSource _image;
        private double _scaleFactor = 1;
        private double _scaleFactorMax = 10;
        private double _scaleFactorMin = 0.1;
        private ObservableCollection<ScanPoint> _items;
        private Project _project;
        private TaskCompletionSource<IProject> _taskCompletionSource;
        private ObservableCollection<NetworkVm> _networks;
        private bool _isModified;
        private ObservableCollection<HeatPoint> _points;

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

        private void OnClick(MouseButtonEventArgs e)
        {

        }

        public ObservableCollection<ScanPoint> Items
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
                if ((value < ScaleFactorMin) || (value > ScaleFactorMax))
                {
                    return;
                }

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
        
        private void OnWheel(MouseWheelEventArgs obj)
        {
            if (Keyboard.PrimaryDevice.IsKeyDown(Key.LeftCtrl) || Keyboard.PrimaryDevice.IsKeyDown(Key.RightCtrl))
            {
                ScaleFactor += ((double)obj.Delta)/100;
                obj.Handled = true;
            }
        }

        public Task Show(Project project, bool isModified, CancellationToken cancellationToken)
        {
            _project = project;
            IsModified = isModified;

            Image = ImageCoder.ByteToImage(_project.Bitmap);
            Items = new ObservableCollection<ScanPoint>(_project.Items);


            var groupBy = Items.SelectMany(point => point.BssInfo).GroupBy(info => info.Ssid);

            var networkVms = groupBy.Select(gr => new NetworkVm(gr.Key, gr.Select(entity => entity.Mac).Distinct()))
                .ToArray();
            
            foreach (var networkVm in networkVms)
            {
                networkVm.PropertyChanged += NetworkVmOnPropertyChanged;
            }

            Networks = new ObservableCollection<NetworkVm>(networkVms);

            cancellationToken.Register(Cancellation);

            _taskCompletionSource = new TaskCompletionSource<IProject>(cancellationToken);

            UpdateHeatMap();

            return _taskCompletionSource.Task;
        }

        private void NetworkVmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateHeatMap();
        }

        private void UpdateHeatMap()
        {
            var heatPoints = new List<HeatPoint>();

            var macs = Networks.SelectMany(vm => vm.Children.Select(i => i.Name)).ToHashSet();
            foreach (var scanPoint in Items)
            {
                var entities = scanPoint.BssInfo.Where(info => macs.Contains(info.Mac)).ToArray();
                if (entities.Length > 0)
                {
                    var max = entities.Max(i => i.Rssi);
                    heatPoints.Add( new HeatPoint(scanPoint.Left, scanPoint.Top, (byte)(max+255)));
                }
            }
            Points = new ObservableCollection<HeatPoint>(heatPoints);
        }

        public ObservableCollection<HeatPoint> Points
        {
            get => _points;
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        private void Cancellation()
        {
            _taskCompletionSource.SetCanceled();
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

        public void Save(string fileName)
        {
            _project.Items = new List<ScanPoint>(Items);
            _project.Save(fileName);
            IsModified = false;
        }
    }
}
