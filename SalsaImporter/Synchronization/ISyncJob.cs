namespace SalsaImporter.Synchronization
{
    public interface ISyncJob
    {
        string Name { get; }
        string ObjectType { get; }
        void Start(IJobContext jobContext);
    }
}
