using System.Collections.Generic;
using System.Linq;
using NativeWifi;
using WiFIMap.Interfaces;

namespace WiFIMap.Network.Wrappers
{
    public class WlanClientWrapper : IWlanClient
    {
        public string GetStringForReasonCode(Wlan.WlanReasonCode reasonCode)
        {
            return _wlanClient.GetStringForReasonCode(reasonCode);
        }

        public IEnumerable<IWlanInterface> Interfaces => _wlanClient.Interfaces.Select(wInterface => new WlanInterfaceWrapper(wInterface));

        private readonly WlanClient _wlanClient;

        public WlanClientWrapper(WlanClient wlanClient)
        {
            _wlanClient = wlanClient;
        }
    }
}