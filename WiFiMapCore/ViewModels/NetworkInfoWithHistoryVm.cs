using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using WiFiMapCore.Interfaces.Network;

namespace WiFiMapCore.ViewModels
{
    public class NetworkInfoWithHistoryVm : BaseVm, INetworkInfo
    {
        private readonly INetworkInfo _networkInfo;
        private int _rssi;
        private uint _linkQuality;
        private PathGeometry _rssiGeometry = new PathGeometry();
        private PathGeometry _linkGeometry = new PathGeometry();
        public string Ssid => _networkInfo.Ssid;

        public string Mac => _networkInfo.Mac;

        public int Rssi
        {
            get => _rssi;
            set
            {
                _rssi = value;
                OnPropertyChanged(nameof(Rssi));
            }
        }

        public uint LinkQuality
        {
            get => _linkQuality;
            set
            {
                _linkQuality = value;
                OnPropertyChanged(nameof(LinkQuality));
            }
        }

        public uint Channel => _networkInfo.Channel;

        public uint ChCenterFrequency => _networkInfo.ChCenterFrequency;
        
        public bool IsConnected { get; }

        public NetworkInfoWithHistoryVm(INetworkInfo networkInfo, bool isConnected)
        {
            _networkInfo = networkInfo;
            IsConnected = isConnected;
            AddRssi(networkInfo.Rssi, networkInfo.LinkQuality);
        }
        
        public LinkedList<int> RssiHistory { get; } = new LinkedList<int>();
        public LinkedList<uint> LinkHistory { get; } = new LinkedList<uint>();

        public PathGeometry RssiGeometry
        {
            get => _rssiGeometry;
            set
            {
                _rssiGeometry = value;
                OnPropertyChanged(nameof(RssiGeometry));
            }
        } 
        
        public PathGeometry LinkGeometry
        {
            get => _linkGeometry;
            set
            {
                _linkGeometry = value;
                OnPropertyChanged(nameof(LinkGeometry));
            }
        }

        public void AddRssi(in int rssi, uint linkQuality)
        {
            RssiHistory.AddFirst(rssi);
            LinkHistory.AddFirst(linkQuality);
            Rssi = rssi;
            LinkQuality = linkQuality;

            while (RssiHistory.Count > 10) RssiHistory.RemoveLast();
            while (LinkHistory.Count > 10) LinkHistory.RemoveLast();

            var width = 5;

            _rssiGeometry.Clear();
            _rssiGeometry.AddGeometry(new LineGeometry(new Point(81,0), new Point(81, 20)));
            using var enumeratorRssi = RssiHistory.GetEnumerator();
            for (int i = 10; i > 0; i--)
            {
                if (!enumeratorRssi.MoveNext())
                    break;
                var histRssi = enumeratorRssi.Current;
                var y = (-histRssi-40)/3;
                var lineGeometry = new LineGeometry(new Point(width*i, y), new Point(width*(i-1), y));
                _rssiGeometry.AddGeometry(lineGeometry);
            }

            _linkGeometry.Clear();
            _linkGeometry.AddGeometry(new LineGeometry(new Point(81,0), new Point(81, 20)));
            using var enumeratorLink = LinkHistory.GetEnumerator();
            for (int i = 10; i > 0; i--)
            {
                if (!enumeratorLink.MoveNext())
                    break;

                var y = ((double) -enumeratorLink.Current + 100) / 100 * 20;
                var lineGeometry = new LineGeometry(new Point(width*i, y), new Point(width*(i-1), y));
                _linkGeometry.AddGeometry(lineGeometry);
            }
        }
    }
}