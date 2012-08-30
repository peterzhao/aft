using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface ISalsaRepository
    {
        
        IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey,
                                                         DateTime minimumModifiedDate);

        void Save(SyncObject syncObject);
        SyncObject Get(string objectType, int key);

        DateTime CurrentTime { get; }

        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}