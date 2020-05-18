using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WiFiMapCore.HeatMap;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Interfaces.Project;

namespace WiFiMapCore.ViewModels
{
    public class HeatPointVm : BaseVm, IHeatPoint, IScanPoint
    {
        public HeatPointVm(Point position, byte intensity, IEnumerable<INetworkInfo> bssInfo)
        {
            Intensity = intensity;
            Position = position;
            BssInfo = bssInfo;
        }

        public HeatPointVm(Point position, IEnumerable<INetworkInfo> bssInfo) : this( position, 0, bssInfo)
        {
            IsMissing = true;
        }

        public bool IsMissing { get; }

        public int X => Position.X;
        public int Y => Position.Y;
        public byte Intensity { get; }
        public Point Position { get; }
        public IEnumerable<INetworkInfo> BssInfo { get; }
    }
}