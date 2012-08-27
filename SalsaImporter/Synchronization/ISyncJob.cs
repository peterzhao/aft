namespace SalsaImporter.Synchronization
{
    public interface ISyncJob
    {
        string Name { get; }
        void Start(IJobContext jobContext);
    }
}
