using System.Collections.Generic;

namespace WiFIMap.Interfaces
{
    public interface INetworkInfo
    {
        IEnumerable<IEntity> GetBssInfo();
        IEnumerable<IEntity> GetBssInfo(string interfaceName);
        IEnumerable<string> GetInterfaces();
    }
}