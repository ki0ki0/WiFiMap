using System.Linq;
using System.Text;
using NativeWifi;
using WiFIMap.Interfaces;

namespace WiFIMap.Network
{
    public class BssEntity : IEntity
    {
        public string Ssid { get; }
        public string Mac { get; }
        public int Rssi { get; }
        public uint LinkQuality { get; }
        public uint Channel { get; }
        public uint ChCenterFrequency { get; }

        public BssEntity(Wlan.WlanBssEntry wlanBssEntry)
        {
            Ssid = Encoding.UTF8.GetString(wlanBssEntry.dot11Ssid.SSID, 0, (int) wlanBssEntry.dot11Ssid.SSIDLength);
            var dot11Bssid = wlanBssEntry.dot11Bssid.Select(i => i.ToString("x2"));
            Mac = string.Join(":", dot11Bssid);
            Rssi = wlanBssEntry.rssi;
            LinkQuality = wlanBssEntry.linkQuality;
            ChCenterFrequency = wlanBssEntry.chCenterFrequency;
            uint channel;
            if (Metadata.ChannelsMap.TryGetValue(ChCenterFrequency/1000, out channel))
            {
                Channel = channel;
            }
        }

        public override string ToString()
        {
            return $"{Ssid}, {Mac}, {Rssi}, {LinkQuality}, {Channel}, {ChCenterFrequency}";
        }
    }
}