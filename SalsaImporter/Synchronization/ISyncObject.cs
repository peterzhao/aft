using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncObject
    {
         int LocalKey { get; set; }
         int? ExternalKey { get; set; }
         DateTime? localModifiedDate { get; set; }
         DateTime? ExternalModifiedDate { get; set; }
        ISyncObject Clone();
    }
}