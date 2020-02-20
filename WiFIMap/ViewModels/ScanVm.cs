﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors.Core;
using WiFIMap.Interfaces;
using WiFIMap.Model;
using WiFIMap.Model.Network;

namespace WiFIMap.ViewModels
{
    public class ScanVm : BaseVm, IProjectContainerVm
    {
        private ImageSource _image;
        private double _scaleFactor = 1;
        private double _scaleFactorMax = 10;
        private double _scaleFactorMin = 0.1;
        private NetworkInfo _networkInfo = new NetworkInfo();
        private ObservableCollection<ScanPoint> _items;
        private Project _project;
        private TaskCompletionSource<IProject> _taskCompletionSource;
        private bool _isModified;

        public ImageSource Image
        {
            get => _image;
            private set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public ICommand Click => new Command<MouseButtonEventArgs>((e) => OnClick(e));

        private async Task OnClick(MouseButtonEventArgs e)
        {
            var inputElement = e.Source as IInputElement;
            var position = e.GetPosition(inputElement);
            //var scanPoint = new ScanPoint(position.X, position.Y);
            //Items.Add(scanPoint);
            var bssInfo = await _networkInfo.GetBssInfo();
            //scanPoint.BssInfo = bssInfo;
            Items.Add(new ScanPoint((int)position.X, (int)position.Y, bssInfo));
            IsModified = true;
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

        public ICommand Finish => new Command<MouseWheelEventArgs>(OnFinish);

        private void OnFinish(MouseWheelEventArgs obj)
        {
            UpdateProject();
            _taskCompletionSource.SetResult(_project);
        }

        private void OnWheel(MouseWheelEventArgs obj)
        {
            if (Keyboard.PrimaryDevice.IsKeyDown(Key.LeftCtrl) || Keyboard.PrimaryDevice.IsKeyDown(Key.RightCtrl))
            {
                ScaleFactor += 0.1 * obj.Delta/Math.Abs(obj.Delta);
                obj.Handled = true;
            }
        }

        public Task Scan(Project project, CancellationToken cancellationToken)
        {
            _project = project;

            Image = ImageCoder.ByteToImage(_project.Bitmap);
            Items = new ObservableCollection<ScanPoint>(_project.Items);

            cancellationToken.Register(Cancellation);

            _taskCompletionSource = new TaskCompletionSource<IProject>(cancellationToken);
            return _taskCompletionSource.Task;
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
            UpdateProject();
            _project.Save(fileName);
            IsModified = false;
        }

        private void UpdateProject()
        {
            _project.Items = new List<ScanPoint>(Items);
        }
    }
}
