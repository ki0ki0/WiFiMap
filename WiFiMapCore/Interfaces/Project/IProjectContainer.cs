namespace WiFiMapCore.Interfaces.Project
{
    public interface IProjectContainer
    {
        bool IsModified { get; }

        void Save(string fileName);
    }
}