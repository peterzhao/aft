using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface IQueueRepository
    {
        void Push(SyncObject syncObject, string tableName);
        event EventHandler<SyncEventArgs> NotifySyncEvent;
        List<SyncObject> DequeueBatchOfObjects(string objectType, string tableName, int batchSize, int startKey);
    }
}