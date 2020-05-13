using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Model.Network.ManagedWiFi;

namespace WiFiMapCore.Model.Network.Wrappers
{
    public class WlanInterfaceWrapper : IWlanInterface
    {
        private readonly WlanClient.WlanInterface _wlanInterface;
        private TaskCompletionSource<bool>? _taskCompletionSource;

        public WlanInterfaceWrapper(WlanClient.WlanInterface wlanInterface)
        {
            _wlanInterface = wlanInterface;
        }

        public Task<bool> Scan()
        {
            _wlanInterface.Scan();
            _taskCompletionSource = new TaskCompletionSource<bool>();
            _wlanInterface.WlanNotification += OnNotification;
            return _taskCompletionSource.Task.ContinueWith(b => {
                _wlanInterface.WlanNotification -= OnNotification;
                return b.Result;
            });
        }

        private void OnNotification(Wlan.WlanNotificationData data)
        {
            if (data.notificationSource == Wlan.WlanNotificationSource.ACM)
            {
                switch (data.notificationCode)
                {
                    case (int) Wlan.WlanNotificationCodeAcm.ScanComplete:
                        _taskCompletionSource?.SetResult(true);
                        break;
                    case (int) Wlan.WlanNotificationCodeAcm.ScanFail:
                        _taskCompletionSource?.SetResult(false);
                        break;
                }
            }
        }

        public Wlan.WlanAvailableNetwork[] GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags flags)
        {
            return _wlanInterface.GetAvailableNetworkList(flags);
        }

        public Wlan.WlanBssEntry[] GetNetworkBssList()
        {
            return _wlanInterface.GetNetworkBssList();
        }

        public Wlan.WlanBssEntry[] GetNetworkBssList(Wlan.Dot11Ssid ssid, Wlan.Dot11BssType bssType,
            bool securityEnabled)
        {
            return _wlanInterface.GetNetworkBssList(ssid, bssType, securityEnabled);
        }

        public void Connect(Wlan.WlanConnectionMode connectionMode, Wlan.Dot11BssType bssType, string profile)
        {
            _wlanInterface.Connect(connectionMode, bssType, profile);
        }

        public bool ConnectSynchronously(Wlan.WlanConnectionMode connectionMode, Wlan.Dot11BssType bssType,
            string profile, int connectTimeout)
        {
            return _wlanInterface.ConnectSynchronously(connectionMode, bssType, profile, connectTimeout);
        }

        public void Connect(Wlan.WlanConnectionMode connectionMode, Wlan.Dot11BssType bssType, Wlan.Dot11Ssid ssid,
            Wlan.WlanConnectionFlags flags)
        {
            _wlanInterface.Connect(connectionMode, bssType, ssid, flags);
        }

        public void DeleteProfile(string profileName)
        {
            _wlanInterface.DeleteProfile(profileName);
        }

        public Wlan.WlanReasonCode SetProfile(Wlan.WlanProfileFlags flags, string profileXml, bool overwrite)
        {
            return _wlanInterface.SetProfile(flags, profileXml, overwrite);
        }

        public string GetProfileXml(string profileName)
        {
            return _wlanInterface.GetProfileXml(profileName);
        }

        public Wlan.WlanProfileInfo[] GetProfiles()
        {
            return _wlanInterface.GetProfiles();
        }

        public bool Autoconf
        {
            get => _wlanInterface.Autoconf;
            set => _wlanInterface.Autoconf = value;
        }

        public Wlan.Dot11BssType BssType
        {
            get => _wlanInterface.BssType;
            set => _wlanInterface.BssType = value;
        }

        public Wlan.WlanInterfaceState InterfaceState => _wlanInterface.InterfaceState;

        public int Channel => _wlanInterface.Channel;

        public int RSSI => _wlanInterface.RSSI;

        public Wlan.Dot11OperationMode CurrentOperationMode => _wlanInterface.CurrentOperationMode;

        public Wlan.WlanConnectionAttributes CurrentConnection => _wlanInterface.CurrentConnection;

        public NetworkInterface NetworkInterface => _wlanInterface.NetworkInterface;

        public Guid InterfaceGuid => _wlanInterface.InterfaceGuid;

        public string InterfaceDescription => _wlanInterface.InterfaceDescription;

        public string InterfaceName => _wlanInterface.InterfaceName;

        public event WlanClient.WlanInterface.WlanNotificationEventHandler WlanNotification
        {
            add => _wlanInterface.WlanNotification += value;
            remove => _wlanInterface.WlanNotification -= value;
        }

        public event WlanClient.WlanInterface.WlanConnectionNotificationEventHandler WlanConnectionNotification
        {
            add => _wlanInterface.WlanConnectionNotification += value;
            remove => _wlanInterface.WlanConnectionNotification -= value;
        }

        public event WlanClient.WlanInterface.WlanReasonNotificationEventHandler WlanReasonNotification
        {
            add => _wlanInterface.WlanReasonNotification += value;
            remove => _wlanInterface.WlanReasonNotification -= value;
        }
    }
}