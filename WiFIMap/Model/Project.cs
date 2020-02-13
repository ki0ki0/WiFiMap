using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WiFIMap.Interfaces;
using WiFIMap.ViewModels;

namespace WiFIMap.Model
{
    public class Project : IProject
    {
        public Project()
        {
        }

        public Project(string fileName)
        {
            var bitmapDecoder = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.None, BitmapCacheOption.Default);
            Bitmap = bitmapDecoder.Frames[0];
            Items = new ObservableCollection<ScanPoint>();
            Items.CollectionChanged += ItemsOnCollectionChanged;
            IsModified = true;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsModified = true;
        }

        public void Load(string fileName)
        {
            OnProjectChanged();
        }

        public void Save(string fileName)
        {
            IsModified = false;
        }

        public ImageSource Bitmap { get; set; }
        public ObservableCollection<ScanPoint> Items { get; set; }
        public bool IsModified { get; set; }
        public event EventHandler<EventArgs> ProjectChanged;

        protected virtual void OnProjectChanged()
        {
            ProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
