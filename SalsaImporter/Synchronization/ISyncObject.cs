namespace SalsaImporter.Synchronization
{
    public interface ISyncObject
    {
         int LocalKey { get; set; }
         int? ExternalKey { get; set; }
    }
}