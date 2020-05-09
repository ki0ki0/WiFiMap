using WiFiMapCore.Interfaces.Network;

namespace WiFiMapCore.Model.Network
{
    public class WifiInterface : IWifiInterface
    {
        public WifiInterface(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}