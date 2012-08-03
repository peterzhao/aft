using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface ISyncObjectRepository
    {
        IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime lastProcessedDateTime) where T:ISyncObject;
        int Add<T>(T syncObject) where T: class, ISyncObject;
        void Update<T>(T newData) where T: class, ISyncObject;
        T GetByExternalKey<T>(int key) where T: class, ISyncObject;
        T Get<T>(int key) where T: class, ISyncObject;

        DateTime CurrentTime { get; }
    }
}