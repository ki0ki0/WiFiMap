using WiFiMapCore.Model.Network;

namespace WiFiMapCore.Interfaces.Network
{
    public interface INetworkInfo
    {
        string Ssid { get; }
        string Mac { get; }
        int Rssi { get; }
        uint LinkQuality { get; }
        uint Channel { get; }
        uint ChCenterFrequency { get; }
    }
}