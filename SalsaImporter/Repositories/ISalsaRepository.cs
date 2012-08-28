using System;
using System.Collections.Generic;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface ISalsaRepository
    {
        
        IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey,
                                                         DateTime minimumModifiedDate);

        int Add(SyncObject syncObject);
        void Update(SyncObject newData);
        SyncObject Get(string objectType, int key);

        DateTime CurrentTime { get; }

        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}