using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeWifi;
using WiFIMap.Interfaces;
using WiFIMap.Model.Network.Wrappers;

namespace WiFIMap.Model.Network
{
    public class NetworkInfo : INetworkInfo
    {
        private readonly IWlanClient _wlanClient = new WlanClientWrapper(new WlanClient());

        public NetworkInfo()
        {
        }

        internal NetworkInfo(IWlanClient wlanClient)
        {
            _wlanClient = wlanClient;
        }

        public IEnumerable<string> GetInterfaces()
        {
            return _wlanClient.Interfaces.Select(GetInterfaceId);
        }

        private string GetInterfaceId(IWlanInterface wlanInterface)
        {
            string wlanInterfaceInterfaceName = null;
            try
            {
                wlanInterfaceInterfaceName = wlanInterface.InterfaceName;
            }
            catch
            {
                // ignored
            }

            string wlanInterfaceInterfaceDescription = null;
            try
            {
                wlanInterfaceInterfaceDescription = wlanInterface.InterfaceDescription;
            }
            catch
            {
                // ignored
            }

            Guid wlanInterfaceInterfaceGuid = Guid.Empty;
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

        public async Task<IEnumerable<IEntity>> GetBssInfo()
        {
            var tasks = _wlanClient.Interfaces.Select(async wlanInterface =>
            {
                wlanInterface.Scan();
                await Task.Delay(TimeSpan.FromSeconds(3));
                var networkBssList = wlanInterface.GetNetworkBssList();
                return networkBssList.Select(entry => new BssEntity(entry));
            });
            var bssEntities = await Task.WhenAll(tasks);
            return bssEntities.SelectMany(items => items);
        }

        public async Task<IEnumerable<IEntity>> GetBssInfo(string interfaceName)
        {
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == interfaceName);

            selectedInterface.Scan();

            await Task.Delay(TimeSpan.FromSeconds(3));


            var networkBssList = selectedInterface.GetNetworkBssList();
            return networkBssList.Select(entry => new BssEntity(entry));
        }
    }
}
