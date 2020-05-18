using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network;

namespace WiFiMapCore.ViewModels
{
    public class NetworkVm : BaseVm
    {
        private ObservableCollection<NetworkVm> _children;
        private readonly INetworkInfo _info;
        private bool? _isChecked = false;
        private string _name = "";

        public NetworkVm(INetworkInfo info)
        {
            _info = info;
            Name = $"{info.Mac}({info.Channel})";
            _children = new ObservableCollection<NetworkVm>();
        }

        public NetworkVm(string name, IEnumerable<NetworkVm> networkVms)
        {
            Name = name;
            _info = new NetworkInfo();
            _children = new ObservableCollection<NetworkVm>(networkVms);
            foreach (var networkVm in _children) networkVm.PropertyChanged += NetworkVmOnPropertyChanged;
        }

        public string Mac => _info.Mac;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool? IsChecked
        {
            get => _isChecked;
            set => UpdateIsChecked(value);
        }
        
        public bool IsExpanded { get; set; }
        
        public ICommand SpacePressed  => new ActionCommand(OnSpacePressed);

        private void OnSpacePressed()
        {
            IsChecked = IsChecked != true;
        }

        private void UpdateIsChecked(bool? value, bool updateChildren = true)
        {
            _isChecked = value;
            OnPropertyChanged(nameof(IsChecked));
            if (!updateChildren) return;
            foreach (var networkVm in _children) networkVm.IsChecked = value;
        }

        public ObservableCollection<NetworkVm> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged(nameof(Children));
            }
        }

        private void NetworkVmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChecked))
            {
                bool? isChecked;
                if (_children.All(i => i.IsChecked == true))
                {
                    isChecked = true;
                }
                else
                {
                    if (_children.Any(i => i.IsChecked == true))
                        isChecked = null;
                    else
                        isChecked = false;
                }
                UpdateIsChecked(isChecked, false);
            }
        }
    }
}