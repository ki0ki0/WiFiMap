using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public IAsyncEnumerable<IWifiInterface> GetInterfaces()
        {
            var networkInterfaces =
                _wlanClient.Interfaces.Select(i => (IWifiInterface) new WifiInterface(GetInterfaceId(i)));
            return networkInterfaces.ToAsyncEnumerable();
        }

        public async Task ForceUpdate()
        {
            await Task.FromResult<object?>(null);
            foreach (var wlanInterface in _wlanClient.Interfaces)
            {
                wlanInterface.Scan();
            }
        }
        public async Task ForceUpdate(IWifiInterface @interface)
        {
            await Task.FromResult<object?>(null);
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == @interface.Name);

            selectedInterface.Scan();
        }

        public async IAsyncEnumerable<INetworkInfo> ReadNetworks(IWifiInterface @interface)
        {
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == @interface.Name);

            await Task.FromResult<object?>(null);
            var networkBssList = selectedInterface.GetNetworkBssList();
            foreach (var entry in networkBssList) yield return new NetworkInfo(entry);
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

        public async IAsyncEnumerable<INetworkInfo> ReadNetworks()
        {
            foreach (var wlanInterface in _wlanClient.Interfaces)
            {
                await Task.FromResult<object?>(null);
                var networkBssList = wlanInterface.GetNetworkBssList();

                foreach (var entry in networkBssList) yield return new NetworkInfo(entry);
            }
        }
    }
}