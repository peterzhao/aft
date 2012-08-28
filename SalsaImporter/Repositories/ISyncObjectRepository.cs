using System;
using System.Collections.Generic;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface ISyncObjectRepository
    {
        
        IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey,
                                                         DateTime minimumModifiedDate); 
       
        int Add<T>(T syncObject) where T : class, ISyncObject;
        void Update<T>(T newData) where T: class, ISyncObject;
        T GetByExternalKey<T>(int key) where T: class, ISyncObject;
        T Get<T>(int key) where T: class, ISyncObject;

        DateTime CurrentTime { get; }

        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}