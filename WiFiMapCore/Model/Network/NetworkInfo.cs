using System;
using System.Linq;
using System.Text;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network.ManagedWiFi;

namespace WiFiMapCore.Model.Network
{
    public class NetworkInfo : INetworkInfo
    {
        public NetworkInfo(Wlan.WlanBssEntry wlanBssEntry)
        {
            Ssid = Encoding.UTF8.GetString(wlanBssEntry.dot11Ssid.SSID, 0, (int) wlanBssEntry.dot11Ssid.SSIDLength);
            var dot11Bssid = wlanBssEntry.dot11Bssid.Select(i => i.ToString("x2"));
            Mac = string.Join(":", dot11Bssid);
            Rssi = wlanBssEntry.rssi;
            LinkQuality = wlanBssEntry.linkQuality;
            ChCenterFrequency = wlanBssEntry.chCenterFrequency;
            uint channel;
            if (Metadata.ChannelsMap.TryGetValue(ChCenterFrequency / 1000, out channel)) Channel = channel;
        }

        public NetworkInfo()
        {
        }

        public string Ssid { get; set; } = string.Empty;
        public string Mac { get; set; } = string.Empty;
        public int Rssi { get; set; }
        public uint LinkQuality { get; set; }
        public uint Channel { get; set; }
        public uint ChCenterFrequency { get; set; }

        protected bool Equals(NetworkInfo other)
        {
            return string.Equals(Ssid, other.Ssid, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Mac, other.Mac, StringComparison.OrdinalIgnoreCase) && Rssi == other.Rssi &&
                   LinkQuality == other.LinkQuality && Channel == other.Channel &&
                   ChCenterFrequency == other.ChCenterFrequency;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NetworkInfo) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Ssid, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(Mac, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(Rssi);
            hashCode.Add(LinkQuality);
            hashCode.Add(Channel);
            hashCode.Add(ChCenterFrequency);
            return hashCode.ToHashCode();
        }
    }
}