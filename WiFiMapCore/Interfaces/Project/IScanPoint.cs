using System.Collections.Generic;
using System.Drawing;
using WiFiMapCore.Interfaces.Network;

namespace WiFiMapCore.Interfaces.Project
{
    public interface IScanPoint
    {
        public Point Position { get; }
        public IEnumerable<INetworkInfo> BssInfo { get; }
    }
}