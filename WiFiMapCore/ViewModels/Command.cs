using System;
using System.Windows.Input;

namespace WiFiMapCore.ViewModels
{
    internal class Command<T> : ICommand where T : class
    {
        private readonly Func<T, bool> _canExecute;

        private readonly Action<T> _execute;

        public Command(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public Command(Action<T> execute) : this(execute, obj => true)
        {
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    internal class AsyncCommand<T> : ICommand where T : class
    {
        private readonly Func<T, bool> _canExecute;

        private readonly Action<T> _execute;

        public AsyncCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public AsyncCommand(Action<T> execute) : this(execute, obj => true)
        {
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}