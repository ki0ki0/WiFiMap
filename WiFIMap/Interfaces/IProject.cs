using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using WiFIMap.ViewModels;

namespace WiFIMap.Interfaces
{
    public interface IProject
    {
        void Load(string fileName);
        void Save(string fileName);

        event EventHandler<EventArgs> ProjectChanged;
        ImageSource Bitmap { get; set; }
        ObservableCollection<ScanPoint> Items { get; set; }
        bool IsModified { get; set; }
    }
}