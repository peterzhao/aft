using System;
using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface ISyncObjectRepository
    {
        IEnumerable<ISyncObject> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime lastProcessedDateTime) where T:ISyncObject;
        ISyncObject Get(int sourceKey);
        int Add(ISyncObject syncObject);
        void Update(ISyncObject newData, ISyncObject oldData);
        ISyncObject GetByExternalKey(int key);
        ISyncObject GetByInternalKey(int key);
    }
}