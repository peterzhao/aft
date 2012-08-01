using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class SalsaRepository : ISyncObjectRepository
    {
        public IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime lastProcessedDateTime) where T : ISyncObject
        {
            throw new NotImplementedException();
        }

        public int Add<T>(T syncObject) where T : ISyncObject
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T newData, T oldData) where T : ISyncObject
        {
            throw new NotImplementedException();
        }

        public T GetByExternalKey<T>(int key) where T:ISyncObject
        {
            throw new NotImplementedException();
        }

        public T GetByLocallKey<T>(int key) where T:ISyncObject
        {
            throw new NotImplementedException();
        }
    }
}
