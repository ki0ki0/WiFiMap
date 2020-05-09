using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WiFiMapCore.Interfaces;

namespace WiFiMapCore.Model.Storage
{
    public class JsonFileStorage<T> : IStorage<T> where T : class, new()
    {
        private readonly string _fileName;

        public JsonFileStorage(string fileName)
        {
            _fileName = fileName;
        }

        public Task Save(T value)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            var serializeObject = JsonConvert.SerializeObject(value, settings);
            return File.WriteAllTextAsync(_fileName, serializeObject);
        }

        public async Task<T> Load()
        {
            var text = await File.ReadAllTextAsync(_fileName);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
                //Formatting = Formatting.Indented
            };
            return JsonConvert.DeserializeObject<T>(text, settings) ?? throw new InvalidOperationException();
        }
    }
}