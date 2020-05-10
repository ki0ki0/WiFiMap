using System;
using System.IO;
using System.Threading;
using System.Windows.Input;

namespace WiFiMapCore.ViewModels
{
    public class ProgressControlVm : BaseVm, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isVisible = true;

        public ICommand Cancel => new Command<MouseEventArgs>(OnCancel);

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private void OnCancel(MouseEventArgs obj)
        {
            _cancellationTokenSource.Cancel();
        }

        public CancellationToken Token => _cancellationTokenSource.Token;

        public void Dispose()
        {
            IsVisible = false;
            _cancellationTokenSource.Cancel();
        }
    }
}