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
using Newtonsoft.Json;
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
            var bitmap = bitmapDecoder.Frames[0];
            Bitmap = ImageCoder.ImageToByte(bitmap);
            Items = new List<ScanPoint>();
        }

        public void Load(string fileName)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var deserializeObject = JsonConvert.DeserializeObject<Project>(File.ReadAllText(fileName), settings);
            Bitmap = deserializeObject.Bitmap;
            Items = new List<ScanPoint>(deserializeObject.Items);
        }

        public void Save(string fileName)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this, settings));
        }
        
        [JsonProperty]
        public List<ScanPoint> Items { get; set; }
        
        [JsonProperty]
        public byte[] Bitmap { get; set; }

    }
}
