using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WiFIMap.Interfaces;

namespace WiFIMap.Model
{
    public class Project : IProject
    {
        public Project(string fileName)
        {
            var bitmapDecoder = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.None, BitmapCacheOption.Default);
            Bitmap = bitmapDecoder.Frames[0];
        }

        public Project()
        {
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> ProjectChanged;
        public ImageSource Bitmap { get; set; }
    }
}
