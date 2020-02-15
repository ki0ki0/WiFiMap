using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using WiFIMap.Model;

namespace WiFIMap.ViewModels
{
    public class MainVm : BaseVm
    {
        private IProjectContainerVm _currentPageViewModel = new BlankVm();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public IProjectContainerVm CurrentPageViewModel
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
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select image with the plan";
            openFileDialog.Filter = "Images|*.jpg;*.png" +
                                    "|All Files|*.*";
            if(openFileDialog.ShowDialog() == true)
            {
                var project = new Project(openFileDialog.FileName);
                _task = StartFlow(project);
            }
        }

        private async Task StartFlow(Project project)
        {
            await StartScan(project);
            await StartShow(project);
        }

        private async Task StartShow(Project project)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var currentPageViewModel = new ResultVm();
            CurrentPageViewModel = currentPageViewModel;
            await currentPageViewModel.Show(project, _cancellationTokenSource.Token);
        }

        private async Task StartScan(Project project)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var currentPageViewModel = new ScanVm();
            CurrentPageViewModel = currentPageViewModel;
            await currentPageViewModel.Scan(project, _cancellationTokenSource.Token);
        }

        public void OnLoadProject(object param)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select project file";
            openFileDialog.Filter = "HeatMap Project|*.heatproject" +
                                    "|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var project = new Project();
                project.Load(openFileDialog.FileName);
                
                _task = StartFlow(project);
            }
        }

        private bool SaveProjectCanExecute(object arg)
        {
            return CurrentPageViewModel.IsModified;
        }

        public void OnSaveProject(object param)
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Title = "Select project file";
            openFileDialog.Filter = "HeatMap Project|*.heatproject" +
                                    "|All Files|*.*";
            if(openFileDialog.ShowDialog() == true)
            {
                CurrentPageViewModel.Save(openFileDialog.FileName);
            }
        }

        public void OnClose(CancelEventArgs e)
        {
            if (CurrentPageViewModel?.IsModified == true)
            {
                if (MessageBox.Show("Exit without saving?", "Exit", MessageBoxButton.YesNoCancel) !=
                    MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            _cancellationTokenSource.Cancel();
            //_task.Wait();
        }

        public void OnExit(object param)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
