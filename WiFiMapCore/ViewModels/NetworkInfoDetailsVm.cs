﻿using WiFiMapCore.Interfaces.Network;

namespace WiFiMapCore.ViewModels
{
    internal class NetworkInfoDetailsVm : INetworkInfo
    {
        private readonly INetworkInfo _networkInfo;

        public NetworkInfoDetailsVm(INetworkInfo networkInfo, bool isSelected, bool isConnected)
        {
            IsSelected = isSelected;
            IsConnected = isConnected;
            _networkInfo = networkInfo;
        }

        public bool IsSelected { get; }
        public bool IsConnected { get; }
        public string Ssid => _networkInfo.Ssid;

        public string Mac => _networkInfo.Mac;

        public int Rssi => _networkInfo.Rssi;

        public uint LinkQuality => _networkInfo.LinkQuality;

        public uint Channel => _networkInfo.Channel;

        public uint ChCenterFrequency => _networkInfo.ChCenterFrequency;
    }
}