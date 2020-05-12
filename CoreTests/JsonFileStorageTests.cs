using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using WiFiMapCore.Model.Network;
using WiFiMapCore.Model.Project;
using WiFiMapCore.Model.Storage;
using Xunit;

namespace CoreTests
{
    public class JsonFileStorageTests
    {
        [Fact]
        public async Task Load_CorrectFileContent()
        {
            var networkInfo = new NetworkInfo();
            networkInfo.ChCenterFrequency = 1;
            networkInfo.Channel = 2;
            networkInfo.LinkQuality = 3;
            networkInfo.Mac = "4";
            networkInfo.Rssi = 5;
            networkInfo.Ssid = "6";

            var scanPoint = new ScanPoint();
            scanPoint.Position = new Point(7, 8);
            scanPoint.BssInfo.Add(networkInfo);

            var expected = new Project();
            expected.Bitmap = new byte[] {9, 10, 11};
            expected.ScanPoints.Add(scanPoint);

            var jsonFileStorage = new JsonFileStorage<Project>("Data\\test.json");
            var actual = await jsonFileStorage.Load();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Save_CorrectFileContent()
        {
            var networkInfo = new NetworkInfo();
            networkInfo.ChCenterFrequency = 1;
            networkInfo.Channel = 2;
            networkInfo.LinkQuality = 3;
            networkInfo.Mac = "4";
            networkInfo.Rssi = 5;
            networkInfo.Ssid = "6";

            var scanPoint = new ScanPoint();
            scanPoint.Position = new Point(7, 8);
            scanPoint.BssInfo.Add(networkInfo);

            var project = new Project();
            project.Bitmap = new byte[] {9, 10, 11};
            project.ScanPoints.Add(scanPoint);

            var tempFileName = Path.GetTempFileName();

            var jsonFileStorage = new JsonFileStorage<Project>(tempFileName);
            await jsonFileStorage.Save(project);

            var actual = await File.ReadAllTextAsync(tempFileName);

            var expected = await File.ReadAllTextAsync("Data\\test.json");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SaveLoad_CorrectProjectContent()
        {
            var random = new Random();

            var networkInfo = new NetworkInfo();
            networkInfo.ChCenterFrequency = (uint) random.Next();
            networkInfo.Channel = (uint) random.Next();
            networkInfo.LinkQuality = (uint) random.Next();
            networkInfo.Mac = Guid.NewGuid().ToString();
            networkInfo.Rssi = random.Next();
            networkInfo.Ssid = Guid.NewGuid().ToString();

            var scanPoint = new ScanPoint();
            scanPoint.Position = new Point(random.Next(), random.Next());
            scanPoint.BssInfo.Add(networkInfo);

            var expected = new Project();
            expected.Bitmap = new byte[100];
            random.NextBytes(expected.Bitmap);
            expected.ScanPoints.Add(scanPoint);

            var tempFileName = Path.GetTempFileName();

            var jsonFileStorage = new JsonFileStorage<Project>(tempFileName);
            await jsonFileStorage.Save(expected);

            var actual = await jsonFileStorage.Load();

            Assert.Equal(expected, actual);
        }
    }
}