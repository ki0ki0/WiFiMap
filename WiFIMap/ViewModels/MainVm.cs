using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WiFIMap.ViewModels
{
    public class MainVm : BaseVm
    {
        private BaseVm _currentPageViewModel;

        public MainVm()
        {
            _currentPageViewModel = new BlankVm();
        }

        public BaseVm CurrentPageViewModel
        {
            get => _currentPageViewModel;
            private set
            {
                _currentPageViewModel = value;
                OnPropertyChanged(nameof(CurrentPageViewModel));
            }
        }

        public ICommand NewProject => new BasicCommand(OnNewProject);
        public ICommand LoadProject => new BasicCommand(OnLoadProject);
        public ICommand SaveProject => new BasicCommand(OnSaveProject, SaveProjectCanExecute);

        public ICommand Exit => new BasicCommand(OnExit);

        public ICommand Close => new Command<CancelEventArgs>(OnClose);


        public void OnNewProject(object param)
        {
            CurrentPageViewModel = new ScanVm();
        }

        public void OnLoadProject(object param)
        {

        }

        private bool SaveProjectCanExecute(object arg)
        {
            return HasCurrentProject;
        }

        private bool HasCurrentProject => !(_currentPageViewModel is BlankVm);

        public void OnSaveProject(object param)
        {

        }

        public void OnClose(CancelEventArgs e)
        {
            if (HasCurrentProject)
            {
                if (MessageBox.Show("Exit without saving?", "Exit", MessageBoxButton.YesNoCancel) !=
                    MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        public void OnExit(object param)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
