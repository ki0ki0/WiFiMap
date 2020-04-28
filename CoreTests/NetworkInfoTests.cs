using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network;
using WiFiMapCore.Model.Network.ManagedWiFi;
using Xunit;

namespace CoreTests
{
    public class NetworkInfoTests
    {
        [Fact]
        public async Task GetBssInfo_NoInterface_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            wlanInterface.InterfaceGuid.Throws(new InvalidOperationException());

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry
            {
                dot11Ssid = new Wlan.Dot11Ssid
                {
                    SSID = ssid,
                    SSIDLength = (uint) ssid.Length
                },
                dot11Bssid = new byte[] {1, 2, 3, 4, 5, 6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var bssInfo = await networkInfo.ReadNetworks().ToListAsync();
            Assert.Single(bssInfo, entity =>
                entity.Ssid == sometext
                && entity.Mac == "01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public async Task GetBssInfo_ValidDescription_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            var interfaceName = Guid.NewGuid().ToString();
            wlanInterface.InterfaceDescription.Returns(interfaceName);
            wlanInterface.InterfaceGuid.Throws(new InvalidOperationException());

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry
            {
                dot11Ssid = new Wlan.Dot11Ssid
                {
                    SSID = ssid,
                    SSIDLength = (uint) ssid.Length
                },
                dot11Bssid = new byte[] {1, 2, 3, 4, 5, 6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var wifiInterface = await networkInfo.GetInterfaces().SingleAsync();
            var bssInfo = await networkInfo.ReadNetworks(wifiInterface).ToListAsync();
            Assert.Single(bssInfo, entity =>
                entity.Ssid == sometext
                && entity.Mac == "01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public async Task GetBssInfo_ValidGuid_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            var interfaceName = Guid.NewGuid();
            wlanInterface.InterfaceGuid.Returns(interfaceName);

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry
            {
                dot11Ssid = new Wlan.Dot11Ssid
                {
                    SSID = ssid,
                    SSIDLength = (uint) ssid.Length
                },
                dot11Bssid = new byte[] {1, 2, 3, 4, 5, 6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var wifiInterface = await networkInfo.GetInterfaces().SingleAsync();
            var bssInfo = await networkInfo.ReadNetworks(wifiInterface).ToListAsync();
            Assert.Single(bssInfo, entity =>
                entity.Ssid == sometext
                && entity.Mac == "01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public async Task GetBssInfo_ValidName_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var interfaceName = Guid.NewGuid().ToString();
            wlanInterface.InterfaceName.Returns(interfaceName);

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry
            {
                dot11Ssid = new Wlan.Dot11Ssid
                {
                    SSID = ssid,
                    SSIDLength = (uint) ssid.Length
                },
                dot11Bssid = new byte[] {1, 2, 3, 4, 5, 6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var wifiInterface = await networkInfo.GetInterfaces().SingleAsync();
            var bssInfo = await networkInfo.ReadNetworks(wifiInterface).ToListAsync();
            Assert.Single(bssInfo, entity =>
                entity.Ssid == sometext
                && entity.Mac == "01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public async void GetInterfaces_HasName_NameIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomName = Guid.NewGuid().ToString();
            wlanInterface.InterfaceName.Returns(randomName);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var interfaces = await networkInfo.GetInterfaces().ToListAsync();
            Assert.Single(interfaces, entity =>
                entity.Name == randomName);
        }

        [Fact]
        public async Task GetInterfaces_NameExceptionAndDescriptionExceptionGuidException_EmptyGuidIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            wlanInterface.InterfaceGuid.Throws(new InvalidOperationException());

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var interfaces = await networkInfo.GetInterfaces().ToListAsync();
            Assert.Single(interfaces, i => i.Name == Guid.Empty.ToString());
        }

        [Fact]
        public async Task GetInterfaces_NameExceptionAndDescriptionExceptionWithGuid_GuidIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomGuid = Guid.NewGuid();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            wlanInterface.InterfaceGuid.Returns(randomGuid);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var interfaces = await networkInfo.GetInterfaces().ToListAsync();
            Assert.Single(interfaces, i => i.Name == randomGuid.ToString());
        }

        [Fact]
        public async Task GetInterfaces_NoNameAndNoDescriptionWithGuid_GuidIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomGuid = Guid.NewGuid();
            wlanInterface.InterfaceName.Returns((string) null);
            wlanInterface.InterfaceDescription.Returns((string) null);
            wlanInterface.InterfaceGuid.Returns(randomGuid);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var interfaces = await networkInfo.GetInterfaces().ToListAsync();
            Assert.Single(interfaces, i => i.Name == randomGuid.ToString());
        }

        [Fact]
        public async Task GetInterfaces_NoNameWithDescription_DescriptionIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomDescription = Guid.NewGuid().ToString();
            wlanInterface.InterfaceName.Returns((string) null);
            wlanInterface.InterfaceDescription.Returns(randomDescription);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworksSource(wlanClient);
            var interfaces = await networkInfo.GetInterfaces().ToListAsync();
            Assert.Single(interfaces, i => i.Name == randomDescription);
        }
    }
}