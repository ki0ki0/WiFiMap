using System;
using System.Windows.Input;

namespace WiFIMap.ViewModels
{
    class Command<T>: ICommand where T:class
    {
        public Command(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public Command(Action<T> execute): this(execute, (obj) => true)
        {
        }

        private Action<T> _execute;

        private Func<T, bool> _canExecute;


        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}