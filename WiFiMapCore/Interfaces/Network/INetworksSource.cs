using System.Collections.Generic;
using System.Threading.Tasks;

namespace WiFiMapCore.Interfaces.Network
{
    public interface INetworksSource
    {
        Task ForceUpdate(IWifiInterface interfaceName);
        Task ForceUpdate();
        IAsyncEnumerable<INetworkInfo> ReadNetworks(IWifiInterface interfaceName);
        IAsyncEnumerable<IWifiInterface> GetInterfaces();
        IAsyncEnumerable<INetworkInfo> ReadNetworks();
    }
}