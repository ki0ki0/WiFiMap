﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors.Core;
using WiFIMap.Interfaces;
using WiFIMap.Model;
using WiFIMap.Network;

namespace WiFIMap.ViewModels
{
    public class ScanVm : BaseVm, IProjectContainer
    {
        private ImageSource _image;
        private double _scaleFactor = 1;
        private double _scaleFactorMax = 10;
        private double _scaleFactorMin = 0.1;
        private NetworkInfo _networkInfo = new NetworkInfo();
        private ObservableCollection<ScanPoint> _items;

        public ScanVm(IProject project)
        {
            CurrentProject = project;
            CurrentProject.ProjectChanged += CurrentProjectOnProjectChanged;
            Load();
        }

        private void Load()
        {
            Image = CurrentProject.Bitmap;
            Items = CurrentProject.Items;
        }

        private void CurrentProjectOnProjectChanged(object sender, EventArgs e)
        {
            Load();
        }

        public IProject CurrentProject { get; }

        public ImageSource Image
        {
            get => _image;
            private set
            {
                _image = value;
                OnPropertyChanged(nameof(CurrentProject));
            }
        }

        public ICommand Click => new Command<MouseButtonEventArgs>(OnClick);

        private void OnClick(MouseButtonEventArgs e)
        {
            var inputElement = e.Source as IInputElement;
            var position = e.GetPosition(inputElement);
            var bssInfo = _networkInfo.GetBssInfo();
            Items.Add(new ScanPoint(position.X, position.Y, bssInfo));
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

        private void OnWheel(MouseWheelEventArgs obj)
        {
            if (Keyboard.PrimaryDevice.IsKeyDown(Key.LeftCtrl) || Keyboard.PrimaryDevice.IsKeyDown(Key.RightCtrl))
            {
                ScaleFactor += ((double)obj.Delta)/100;
                obj.Handled = true;
            }
        }
    }
}
