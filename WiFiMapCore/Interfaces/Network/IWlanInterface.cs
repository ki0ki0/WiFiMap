using System;
using System.Net.NetworkInformation;
using WiFiMapCore.Model.Network.ManagedWiFi;

namespace WiFiMapCore.Interfaces.Network
{
    public interface IWlanInterface
    {
        bool Autoconf { get; set; }
        Wlan.Dot11BssType BssType { get; set; }
        int Channel { get; }
        Wlan.WlanConnectionAttributes CurrentConnection { get; }
        Wlan.Dot11OperationMode CurrentOperationMode { get; }
        string InterfaceDescription { get; }
        Guid InterfaceGuid { get; }
        string InterfaceName { get; }
        Wlan.WlanInterfaceState InterfaceState { get; }
        NetworkInterface NetworkInterface { get; }
        int RSSI { get; }

        event WlanClient.WlanInterface.WlanConnectionNotificationEventHandler WlanConnectionNotification;
        event WlanClient.WlanInterface.WlanNotificationEventHandler WlanNotification;
        event WlanClient.WlanInterface.WlanReasonNotificationEventHandler WlanReasonNotification;

        void Connect(Wlan.WlanConnectionMode connectionMode, Wlan.Dot11BssType bssType, Wlan.Dot11Ssid ssid,
            Wlan.WlanConnectionFlags flags);

        void Connect(Wlan.WlanConnectionMode connectionMode, Wlan.Dot11BssType bssType, string profile);

        bool ConnectSynchronously(Wlan.WlanConnectionMode connectionMode, Wlan.Dot11BssType bssType, string profile,
            int connectTimeout);

        void DeleteProfile(string profileName);
        Wlan.WlanAvailableNetwork[] GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags flags);
        Wlan.WlanBssEntry[] GetNetworkBssList();
        Wlan.WlanBssEntry[] GetNetworkBssList(Wlan.Dot11Ssid ssid, Wlan.Dot11BssType bssType, bool securityEnabled);
        Wlan.WlanProfileInfo[] GetProfiles();
        string GetProfileXml(string profileName);
        void Scan();
        Wlan.WlanReasonCode SetProfile(Wlan.WlanProfileFlags flags, string profileXml, bool overwrite);
    }
}