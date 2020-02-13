using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;
using WiFIMap;
using WiFIMap.Interfaces;
using WiFIMap.Network;
using Xunit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Tests
{
    public class NetworkInfoTests
    {
        [Fact]
        public void GetInterfaces_HasName_NameIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomName = Guid.NewGuid().ToString();
            wlanInterface.InterfaceName.Returns(randomName);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var bssInfo = networkInfo.GetInterfaces();
            Assert.Single(bssInfo, randomName);
        }

        [Fact]
        public void GetInterfaces_NoNameWithDescription_DescriptionIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomDescription = Guid.NewGuid().ToString();
            wlanInterface.InterfaceName.Returns((string)null);
            wlanInterface.InterfaceDescription.Returns(randomDescription);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var interfaces = networkInfo.GetInterfaces();
            Assert.Single(interfaces, randomDescription);
        }

        [Fact]
        public void GetInterfaces_NoNameAndNoDescriptionWithGuid_GuidIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomGuid = Guid.NewGuid();
            wlanInterface.InterfaceName.Returns((string)null);
            wlanInterface.InterfaceDescription.Returns((string)null);
            wlanInterface.InterfaceGuid.Returns(randomGuid);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var interfaces = networkInfo.GetInterfaces();
            Assert.Single(interfaces, randomGuid.ToString());
        }

        [Fact]
        public void GetInterfaces_NameExceptionAndDescriptionExceptionWithGuid_GuidIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var randomGuid = Guid.NewGuid();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            wlanInterface.InterfaceGuid.Returns(randomGuid);

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var interfaces = networkInfo.GetInterfaces();
            Assert.Single(interfaces, randomGuid.ToString());
        }

        [Fact]
        public void GetInterfaces_NameExceptionAndDescriptionExceptionGuidException_EmptyGuidIsReturned()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            wlanInterface.InterfaceGuid.Throws(new InvalidOperationException());

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var interfaces = networkInfo.GetInterfaces();
            Assert.Single(interfaces, Guid.Empty.ToString());
        }
        
        [Fact]
        public void GetBssInfo_ValidName_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            var interfaceName = Guid.NewGuid().ToString();
            wlanInterface.InterfaceName.Returns(interfaceName);

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry()
            {
                dot11Ssid = new Wlan.Dot11Ssid()
                {
                    SSID = ssid,
                    SSIDLength = (uint)ssid.Length
                },
                dot11Bssid = new byte[]{1,2,3,4,5,6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var bssInfo = networkInfo.GetBssInfo(interfaceName);
            Assert.Single(bssInfo, entity => 
                entity.Ssid == sometext 
                && entity.Mac =="01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public void GetBssInfo_ValidDescription_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            var interfaceName = Guid.NewGuid().ToString();
            wlanInterface.InterfaceDescription.Returns(interfaceName);
            wlanInterface.InterfaceGuid.Throws(new InvalidOperationException());
            
            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry()
            {
                dot11Ssid = new Wlan.Dot11Ssid()
                {
                    SSID = ssid,
                    SSIDLength = (uint)ssid.Length
                },
                dot11Bssid = new byte[]{1,2,3,4,5,6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var bssInfo = networkInfo.GetBssInfo(interfaceName);
            Assert.Single(bssInfo, entity => 
                entity.Ssid == sometext 
                && entity.Mac =="01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public void GetBssInfo_ValidGuid_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            var interfaceName = Guid.NewGuid();
            wlanInterface.InterfaceGuid.Returns(interfaceName);

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry()
            {
                dot11Ssid = new Wlan.Dot11Ssid()
                {
                    SSID = ssid,
                    SSIDLength = (uint)ssid.Length
                },
                dot11Bssid = new byte[]{1,2,3,4,5,6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var bssInfo = networkInfo.GetBssInfo(interfaceName.ToString());
            Assert.Single(bssInfo, entity => 
                entity.Ssid == sometext 
                && entity.Mac =="01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }

        [Fact]
        public void GetBssInfo_NoInterface_ValidList()
        {
            var wlanInterface = Substitute.For<IWlanInterface>();
            wlanInterface.InterfaceName.Throws(new InvalidOperationException());
            wlanInterface.InterfaceDescription.Throws(new InvalidOperationException());
            wlanInterface.InterfaceGuid.Throws(new InvalidOperationException());

            var sometext = "sometext";
            var ssid = Encoding.UTF8.GetBytes(sometext);
            var wlanBssEntry = new Wlan.WlanBssEntry()
            {
                dot11Ssid = new Wlan.Dot11Ssid()
                {
                    SSID = ssid,
                    SSIDLength = (uint)ssid.Length
                },
                dot11Bssid = new byte[]{1,2,3,4,5,6},
                rssi = 7,
                linkQuality = 8,
                chCenterFrequency = 2422000
            };
            wlanInterface.GetNetworkBssList().Returns(new[] {wlanBssEntry});

            var wlanClient = Substitute.For<IWlanClient>();
            wlanClient.Interfaces.Returns(new[] {wlanInterface});
            var networkInfo = new NetworkInfo(wlanClient);
            var bssInfo = networkInfo.GetBssInfo();
            Assert.Single(bssInfo, entity => 
                entity.Ssid == sometext 
                && entity.Mac =="01:02:03:04:05:06"
                && entity.Rssi == wlanBssEntry.rssi
                && entity.LinkQuality == wlanBssEntry.linkQuality
                && entity.ChCenterFrequency == wlanBssEntry.chCenterFrequency
                && entity.Channel == 3);
        }
    }
}
