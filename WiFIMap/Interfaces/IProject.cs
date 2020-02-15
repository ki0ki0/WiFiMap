using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WiFIMap.ViewModels;

namespace WiFIMap.Interfaces
{
    public interface IProject
    {
        void Load(string fileName);
        void Save(string fileName);

        byte[] Bitmap { get; set; }
        List<ScanPoint> Items { get; set; }
    }
}