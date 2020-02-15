namespace WiFIMap.Interfaces
{
    public interface IProjectContainer
    {
        bool IsModified { get; }
        
        void Save(string fileName);
    }
}