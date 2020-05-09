using System.Threading.Tasks;

namespace WiFiMapCore.Interfaces
{
    public interface IStorage<T>
    {
        Task Save(T value);
        Task<T> Load();
    }
}