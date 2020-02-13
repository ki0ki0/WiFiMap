using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
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
                if (CurrentPageViewModel is ScanVm scanVm)
                {
                    scanVm.CurrentProject.Save(openFileDialog.FileName);
                }
                else
                {
                    if (CurrentPageViewModel is ResultVm resultVm)
                    {
                        resultVm.CurrentProject.Save(openFileDialog.FileName);
                    }
                    else
                    {
                        var project = new Project();
                        project.Load(openFileDialog.FileName);
                        CurrentPageViewModel = new ScanVm(project);
                    }
                }
            }
        }

        private bool SaveProjectCanExecute(object arg)
        {
            return HasCurrentProject;
        }

        private bool HasCurrentProject => !(_currentPageViewModel is BlankVm);

        public void OnSaveProject(object param)
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Title = "Select project file";
            openFileDialog.Filter = "HeatMap Project|*.heatproject" +
                                    "|All Files|*.*";
            if(openFileDialog.ShowDialog() == true)
            {
                if (CurrentPageViewModel is ScanVm scanVm)
                {
                    scanVm.CurrentProject.Save(openFileDialog.FileName);
                }
                else
                {
                    if (CurrentPageViewModel is ResultVm resultVm)
                    {
                        resultVm.CurrentProject.Save(openFileDialog.FileName);
                    }
                    else
                    {
                        throw new NotImplementedException("You can save only from scan or result mode.");
                    }
                }
            }
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
