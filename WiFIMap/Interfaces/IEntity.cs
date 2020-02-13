namespace WiFIMap.Interfaces
{
    public interface IEntity
    {
        string Ssid { get; }
        string Mac { get; }
        int Rssi { get; }
        uint LinkQuality { get; }
        uint Channel { get; }
        uint ChCenterFrequency { get; }
    }
}