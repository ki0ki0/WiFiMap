using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WiFiMapCore.Interfaces.Network;
using WiFiMapCore.Interfaces.Project;

namespace WiFiMapCore.Model.Project
{
    public class ScanPoint : IScanPoint
    {
        public List<INetworkInfo> BssInfo { get; } = new List<INetworkInfo>();
        public Point Position { get; set; }

        IEnumerable<INetworkInfo> IScanPoint.BssInfo => BssInfo;

        public ScanPoint()
        {
        }

        public ScanPoint(int x, int y, IEnumerable<INetworkInfo> infos)
        {
            Position = new Point(x, y);
            BssInfo.AddRange(infos);
        }

        protected bool Equals(ScanPoint other)
        {
            return BssInfo.SequenceEqual(other.BssInfo) && Position.Equals(other.Position);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ScanPoint) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BssInfo, Position);
        }
    }
}