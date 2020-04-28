using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WiFiMapCore.Interfaces;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Project;
using WiFiMapCore.Model.Storage;

namespace WiFiMapCore.ViewModels
{
    public class MainVm : BaseVm
    {
        public ICommand NewProject => new BasicCommand(OnNewProject);
        public ICommand LoadProject => new BasicCommand(OnLoadProject);
        public ICommand SaveProject => new BasicCommand(OnSaveProject, SaveProjectCanExecute);

        public ICommand Exit => new BasicCommand(OnExit);

        public ICommand Close => new Command<CancelEventArgs>(OnClose);

        public ProjectVm ProjectVm { get; } = new ProjectVm();


        public void OnNewProject(object param)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select image with the plan";
            openFileDialog.Filter = "Images|*.jpg;*.png" +
                                    "|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var project = new Project();
                var bitmapDecoder = BitmapDecoder.Create(new Uri(openFileDialog.FileName), BitmapCreateOptions.None,
                    BitmapCacheOption.Default);
                var bitmap = bitmapDecoder.Frames[0];
                project.Bitmap = ImageCoder.ImageToByte(bitmap);
                ProjectVm.Project = project;
            }
        }

        public async void OnLoadProject(object param)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select project file";
            openFileDialog.Filter = "HeatMap Project|*.heatproject" +
                                    "|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                IStorage<Project> str = new JsonFileStorage<Project>(openFileDialog.FileName);
                var project = await str.Load();
                ProjectVm.Project = project;
            }
        }

        private bool SaveProjectCanExecute(object arg)
        {
            return ProjectVm.IsModified;
        }

        public async void OnSaveProject(object param)
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Title = "Select project file";
            openFileDialog.Filter = "HeatMap Project|*.heatproject" +
                                    "|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                IStorage<Project> str = new JsonFileStorage<Project>(openFileDialog.FileName);
                await str.Save(ProjectVm.Project);
                ProjectVm.IsModified = false;
            }
        }

        public void OnClose(CancelEventArgs e)
        {
            if (ProjectVm?.IsModified == true)
                if (MessageBox.Show("Exit without saving?", "Exit", MessageBoxButton.YesNoCancel) !=
                    MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }

            //_task.Wait();
        }

        public void OnExit(object param)
        {
            Application.Current.MainWindow.Close();
        }
    }
}