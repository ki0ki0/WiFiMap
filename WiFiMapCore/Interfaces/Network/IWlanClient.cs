using System.Collections.Generic;
using WiFiMapCore.Model.Network.ManagedWiFi;

namespace WiFiMapCore.Interfaces.Network
{
    public interface IWlanClient
    {
        IEnumerable<IWlanInterface> Interfaces { get; }

        string GetStringForReasonCode(Wlan.WlanReasonCode reasonCode);
    }
}