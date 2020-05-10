using System.Collections.Generic;

namespace WiFiMapCore.Interfaces.Network
{
    public class MacEqualityComparer : IEqualityComparer<INetworkInfo>
    {
        public bool Equals(INetworkInfo? x, INetworkInfo? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Mac == y.Mac;
        }

        public int GetHashCode(INetworkInfo? obj)
        {
            return obj?.Mac.GetHashCode() ?? 0;
        }
    }
}