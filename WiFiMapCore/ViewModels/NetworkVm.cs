using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network;

namespace WiFiMapCore.ViewModels
{
    public class NetworkVm : BaseVm
    {
        private ObservableCollection<NetworkVm> _children;
        private bool _ignoreChildren;
        private bool? _isChecked = false;
        private string _name = "";
        private INetworkInfo _info;
        public string Mac => _info.Mac;

        public NetworkVm(INetworkInfo info)
        {
            _info = info;
            Name = $"{info.Mac}({info.Channel})";
            _children = new ObservableCollection<NetworkVm>();
        }

        public NetworkVm(string name, IEnumerable<INetworkInfo> macs)
        {
            Name = name;
            _info = new NetworkInfo();
            var networkVms = macs
                .OrderBy(info => info.Channel)
                .ThenBy(info => info.Mac)
                .Select(mac => new NetworkVm(mac));
            _children = new ObservableCollection<NetworkVm>(networkVms);
            foreach (var networkVm in _children) networkVm.PropertyChanged += NetworkVmOnPropertyChanged;
        }

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
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
                if (_ignoreChildren) return;
                foreach (var networkVm in _children) networkVm.IsChecked = value;
            }
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
            _ignoreChildren = true;
            if (e.PropertyName == nameof(IsChecked))
            {
                if (_children.All(i => i.IsChecked == true))
                {
                    IsChecked = true;
                }
                else
                {
                    if (_children.Any(i => i.IsChecked == true))
                        IsChecked = null;
                    else
                        IsChecked = false;
                }
            }

            _ignoreChildren = false;
        }
    }
}