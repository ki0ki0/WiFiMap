﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WiFiMapCore.Interfaces;
using WiFiMapCore.Model;
using WiFiMapCore.Model.Project;
using WiFiMapCore.Model.Storage;
using WiFiMapCore.Views;

namespace WiFiMapCore.ViewModels
{
    public class MainVm : BaseVm
    {
        public MainVm()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length == 2)
            {
                var load = Load(commandLineArgs[1]);
            }
        }

        public ICommand NewProject => new BasicCommand(OnNewProject);
        public ICommand LoadProject => new BasicCommand(OnLoadProject);
        public ICommand SaveProject => new BasicCommand(OnSaveProject, SaveProjectCanExecute);
        public ICommand Exit => new BasicCommand(OnExit);
        public ICommand Close => new Command<CancelEventArgs>(OnClose);
        public ICommand Diagnostics => new Command<CancelEventArgs>(OnDiagnostics);

        public ScanVm ScanVm { get; } = new ScanVm();

        private void OnDiagnostics(CancelEventArgs obj)
        {
            var diagnosticsView = new DiagnosticsView();
            diagnosticsView.ShowDialog();
        }

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
                ScanVm.Project = project;
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
                var fileName = openFileDialog.FileName;
                await Load(fileName);
            }
        }

        private async Task Load(string fileName)
        {
            IStorage<Project> str = new JsonFileStorage<Project>(fileName);
            var project = await str.Load();
            ScanVm.Project = project;
        }

        private bool SaveProjectCanExecute(object arg)
        {
            return ScanVm.IsModified;
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
                await str.Save(ScanVm.Project);
                ScanVm.IsModified = false;
            }
        }

        public void OnClose(CancelEventArgs e)
        {
            if (ScanVm?.IsModified == true)
                if (MessageBox.Show("Exit without saving?", "Exit", MessageBoxButton.YesNoCancel) !=
                    MessageBoxResult.Yes)
                    e.Cancel = true;

            //_task.Wait();
        }

        public void OnExit(object param)
        {
            Application.Current.MainWindow.Close();
        }
    }
}