using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class BatchOneWaySynchronizer :ISyncJob
    {
        private readonly ISyncObjectRepository _source;
        private readonly IConditonalUpdater _destination;
        private readonly IJobContext _jobContext;
        private int _batchSize;

        public BatchOneWaySynchronizer(ISyncObjectRepository source, IConditonalUpdater destination, IJobContext jobContext, int batchSize)
        {
            _source = source;
            _destination = destination;
            _jobContext = jobContext;
            _batchSize = batchSize;
        }

        public void Synchronize<T>() where T:class, ISyncObject
        {
            IEnumerable<T> currentBatch;
            do
            {
                currentBatch = _source.GetBatchOfObjects<T>(_batchSize, 
                    _jobContext.CurrentRecord, 
                    _jobContext.MinimumModificationDate);
                var tasks = currentBatch.Select(obj => Task.Factory.StartNew(arg => _destination.MaybeUpdate<T>(obj), null));
                Task.WaitAll(tasks.ToArray());

                if (currentBatch.Any())
                {
                    _jobContext.CurrentRecord = currentBatch.Last().Id;
                }

            } while (currentBatch.Count() >= _batchSize);
           
            _jobContext.MarkComplete();
        }

        public string Name { get; private set; }
        public void Start(IJobContext jobContext)
        {
            throw new NotImplementedException();
        }
    }
}