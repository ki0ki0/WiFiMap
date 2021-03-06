﻿using System.Collections.Generic;
using System.Linq;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network.ManagedWiFi;

namespace WiFiMapCore.Model.Network.Wrappers
{
    public class WlanClientWrapper : IWlanClient
    {
        private readonly WlanClient _wlanClient;

        public WlanClientWrapper(WlanClient wlanClient)
        {
            _wlanClient = wlanClient;
        }

        public string GetStringForReasonCode(Wlan.WlanReasonCode reasonCode)
        {
            return _wlanClient.GetStringForReasonCode(reasonCode);
        }

        public IEnumerable<IWlanInterface> Interfaces =>
            _wlanClient.Interfaces.Select(wInterface => new WlanInterfaceWrapper(wInterface));
    }
}