using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncObject
    {
         int LocalKey { get; set; }
         int? ExternalKey { get; set; }
         DateTime? LocalModifiedDate { get; }
         DateTime? ExternalModifiedDate { get; set; }
         ISyncObject Clone();
    }
}