using System;
using System.IO;
using System.Windows.Media;

namespace WiFIMap.Interfaces
{
    public interface IProject
    {
        void Load(string fileName);
        void Save(string fileName);

        event EventHandler<EventArgs> ProjectChanged;
        ImageSource Bitmap { get; set; }
    }
}