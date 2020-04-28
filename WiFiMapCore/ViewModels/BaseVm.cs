using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WiFiMapCore.ViewModels
{
    public class BaseVm : INotifyPropertyChanged
    {
        private const string StringEmpty = "";

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = StringEmpty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}