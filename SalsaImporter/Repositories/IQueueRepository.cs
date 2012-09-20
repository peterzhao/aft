using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface IQueueRepository
    {
        void Enqueue(string tableName, SyncObject syncObject);
        List<SyncObject> GetBatchOfObjects(string objectType, string tableName, int batchSize, int startKey);
        void Dequeue(string tableName, int id);
        void UpdateStatus(string tableName, int id, string status, DateTime? processedDate = null);
    }
}