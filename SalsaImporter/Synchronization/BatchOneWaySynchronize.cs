using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class BatchOneWaySynchronize
    {
        private readonly ISyncObjectRepository _source;
        private readonly IConditonalUpdater _destination;
        private readonly ISyncState _syncState;
        private int _batchSize;

        public BatchOneWaySynchronize(ISyncObjectRepository source, IConditonalUpdater destination, ISyncState syncState, int batchSize)
        {
            _source = source;
            _destination = destination;
            _syncState = syncState;
            _batchSize = batchSize;
        }

        public void Synchronize<T>() where T:class, ISyncObject 
        {
            var minimumModificationDate = _syncState.MinimumModificationDate;
            do
            {
                IEnumerable<T> currentBatch = _source.GetBatchOfObjects<T>(_batchSize, _syncState.CurrentRecord, minimumModificationDate);
                var tasks = currentBatch.Select(obj => Task.Factory.StartNew(arg => _destination.MaybeUpdate<T>(obj), null));
                Task.WaitAll(tasks.ToArray());
                if (!currentBatch.Any())
                {
                   _syncState.MarkComplete(); 
                   break;
                }
                _syncState.CurrentRecord = currentBatch.Last().ExternalKey.Value;

            } while (true);
        }
    }
}