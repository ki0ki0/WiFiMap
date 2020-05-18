using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WiFiMapCore.Interfaces.Network
{
    public interface INetworksSource
    {
        IAsyncEnumerable<IWifiInterface> GetInterfaces(CancellationToken token = default);
        Task ForceUpdate(CancellationToken token = default);
        Task ForceUpdate(IWifiInterface interfaceName, CancellationToken token = default);
        IAsyncEnumerable<INetworkInfo> ReadNetworks(CancellationToken token = default);
        IAsyncEnumerable<INetworkInfo> ReadNetworks(IWifiInterface interfaceName, CancellationToken token = default);
        IAsyncEnumerable<string> GetConnected(CancellationToken token = default);
        Task<string> GetConnected(IWifiInterface interfaceName, CancellationToken token = default);
    }
}