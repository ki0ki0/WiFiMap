using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network.ManagedWiFi;
using WiFiMapCore.Model.Network.Wrappers;

namespace WiFiMapCore.Model.Network
{
    public class NetworksSource : INetworksSource
    {
        private readonly IWlanClient _wlanClient = new WlanClientWrapper(new WlanClient());

        public NetworksSource()
        {
        }

        internal NetworksSource(IWlanClient wlanClient)
        {
            _wlanClient = wlanClient;
        }

        public IAsyncEnumerable<IWifiInterface> GetInterfaces(CancellationToken token = default)
        {
            var networkInterfaces =
                _wlanClient.Interfaces.Select(i => (IWifiInterface) new WifiInterface(GetInterfaceId(i)));
            return networkInterfaces.ToAsyncEnumerable();
        }

        public async Task ForceUpdate(CancellationToken token = default)
        {
            var tasks = _wlanClient.Interfaces.Select(@interface => @interface.Scan(token));
            await Task.WhenAll(tasks);
        }

        public async Task ForceUpdate(IWifiInterface @interface, CancellationToken token = default)
        {
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == @interface.Name);

            await selectedInterface.Scan(token);
        }

        public async IAsyncEnumerable<INetworkInfo> ReadNetworks(
            [EnumeratorCancellation] CancellationToken token = default)
        {
            foreach (var wlanInterface in _wlanClient.Interfaces)
            {
                await Task.FromResult<object?>(null);
                var networkBssList = wlanInterface.GetNetworkBssList();

                foreach (var entry in networkBssList)
                {
                    token.ThrowIfCancellationRequested();
                    yield return new NetworkInfo(entry);
                }
            }
        }

        public async IAsyncEnumerable<INetworkInfo> ReadNetworks(IWifiInterface @interface,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == @interface.Name);

            await Task.FromResult<object?>(null);
            var networkBssList = selectedInterface.GetNetworkBssList();
            foreach (var entry in networkBssList)
            {
                token.ThrowIfCancellationRequested();
                yield return new NetworkInfo(entry);
            }
        }

        public IAsyncEnumerable<string> GetConnected(CancellationToken token = default)
        {
            return _wlanClient.Interfaces.Select(wlanInterface =>
                    NetworkInfo.BssidToMac(wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Bssid))
                .ToAsyncEnumerable();
        }
        
        public async Task<string> GetConnected(IWifiInterface @interface, CancellationToken token = default)
        {
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == @interface.Name);

            await Task.FromResult<object?>(null);
            return NetworkInfo.BssidToMac(selectedInterface.CurrentConnection.wlanAssociationAttributes.dot11Bssid);
        }

        private string GetInterfaceId(IWlanInterface wlanInterface)
        {
            string? wlanInterfaceInterfaceName = null;
            try
            {
                wlanInterfaceInterfaceName = wlanInterface.InterfaceName;
            }
            catch
            {
                // ignored
            }

            string? wlanInterfaceInterfaceDescription = null;
            try
            {
                wlanInterfaceInterfaceDescription = wlanInterface.InterfaceDescription;
            }
            catch
            {
                // ignored
            }

            var wlanInterfaceInterfaceGuid = Guid.Empty;
            try
            {
                wlanInterfaceInterfaceGuid = wlanInterface.InterfaceGuid;
            }
            catch
            {
                // ignored
            }

            return wlanInterfaceInterfaceName ??
                   wlanInterfaceInterfaceDescription ?? wlanInterfaceInterfaceGuid.ToString();
        }
    }
}