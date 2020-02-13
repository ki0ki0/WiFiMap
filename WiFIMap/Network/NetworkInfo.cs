using System;
using System.Collections.Generic;
using System.Linq;
using NativeWifi;
using WiFIMap.Interfaces;
using WiFIMap.Network.Wrappers;

namespace WiFIMap.Network
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

        public IEnumerable<IEntity> GetBssInfo()
        {
            return _wlanClient.Interfaces.SelectMany(wlanInterface =>
            {
                var networkBssList = wlanInterface.GetNetworkBssList();
                return networkBssList.Select(entry => new BssEntity(entry));
            });
        }

        public IEnumerable<IEntity> GetBssInfo(string interfaceName)
        {
            var selectedInterface =
                _wlanClient.Interfaces.Single(wlanInterface => GetInterfaceId(wlanInterface) == interfaceName);

            var networkBssList = selectedInterface.GetNetworkBssList();
            return networkBssList.Select(entry => new BssEntity(entry));
        }
    }
}
