

namespace SalsaImporter.Synchronization
{
    public interface IObjectUpdater
    {
        void Update<T>(T sourceObject) where T : class, ISyncObject;
    }
}