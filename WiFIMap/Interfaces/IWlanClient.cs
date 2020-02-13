using System.Collections.Generic;
using NativeWifi;

namespace WiFIMap.Interfaces
{
    public interface IWlanClient
    {
        IEnumerable<IWlanInterface> Interfaces { get; }

        string GetStringForReasonCode(Wlan.WlanReasonCode reasonCode);
    }
}