using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WiFiMapCore.ViewModels
{
    internal class AsyncCommand<T> : ICommand where T : class
    {
        private readonly Func<T, bool> _canExecute;

        private readonly Func<T, Task> _execute;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public AsyncCommand(Func<T, Task> execute) : this(execute, obj => true)
        {
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public async void Execute(object parameter)
        {
            try
            {
                await _execute((T) parameter);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}