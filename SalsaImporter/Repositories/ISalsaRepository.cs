using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface ISalsaRepository
    {
        IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey, DateTime minimumModifiedDate);
        bool Save(SyncObject syncObject); //return true: saved; false: skipped because the save value;
        SyncObject Get(string objectType, int key);
        DateTime CurrentTime { get; }

    }
}