using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace WiFiMapCore.ViewModels
{
    public class NetworkVm : BaseVm
    {
        private ObservableCollection<NetworkVm> _children;
        private bool _ignoreChildren;
        private bool? _isChecked = true;
        private string _name = "";

        public NetworkVm(string name)
        {
            Name = name;
            _children = new ObservableCollection<NetworkVm>();
        }

        public NetworkVm(string name, IEnumerable<string> macs)
        {
            Name = name;
            _children = new ObservableCollection<NetworkVm>(macs.Select(mac => new NetworkVm(mac)));
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