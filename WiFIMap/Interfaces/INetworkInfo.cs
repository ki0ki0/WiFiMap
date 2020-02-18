using System.Collections.Generic;
using System.Threading.Tasks;

namespace WiFIMap.Interfaces
{
    public interface INetworkInfo
    {
        Task<IEnumerable<IEntity>> GetBssInfo();
        Task<IEnumerable<IEntity>> GetBssInfo(string interfaceName);
        IEnumerable<string> GetInterfaces();
    }
}