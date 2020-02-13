using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using WiFIMap.Interfaces;
using WiFIMap.Model;

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
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select image with the plan";
            openFileDialog.Filter = "Images|*.jpg;*.png" +
                                    "|All Files|*.*";
            if(openFileDialog.ShowDialog() == true)
            {
                var project = new Project(openFileDialog.FileName);
                CurrentPageViewModel = new ScanVm(project);
            }
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
                CurrentPageViewModel = new ScanVm(project);
            }
        }

        private IProject GetCurrentProject()
        {
            switch (CurrentPageViewModel)
            {
                case ScanVm scanVm:
                    return scanVm.CurrentProject;
                case ResultVm resultVm:
                    return resultVm.CurrentProject;
                default:
                {
                    return null;
                }
            }
        }

        private bool SaveProjectCanExecute(object arg)
        {
            return GetCurrentProject() != null;
        }

        public void OnSaveProject(object param)
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Title = "Select project file";
            openFileDialog.Filter = "HeatMap Project|*.heatproject" +
                                    "|All Files|*.*";
            if(openFileDialog.ShowDialog() == true)
            {
                var currentProject = GetCurrentProject();
                currentProject.Save(openFileDialog.FileName);
            }
        }

        public void OnClose(CancelEventArgs e)
        {
            var currentProject = GetCurrentProject();
            if (currentProject?.IsModified == true)
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
