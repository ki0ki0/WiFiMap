using System.Collections.Generic;

namespace WiFiMapCore.Interfaces.Network
{
    public interface INetworksSource
    {
        IAsyncEnumerable<INetworkInfo> ReadNetworks(IWifiInterface interfaceName);
        IAsyncEnumerable<IWifiInterface> GetInterfaces();
    }
}