using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class BatchOneWaySyncJob<T> :ISyncJob where T: class , ISyncObject
    {
        private readonly ISyncObjectRepository _source;
        private readonly IConditonalUpdater _destination;
        private int _batchSize;

        public string Name { get; set; }


        public BatchOneWaySyncJob(ISyncObjectRepository source, IConditonalUpdater destination, int batchSize, string name)
        {
            Name = name;
            _source = source;
            _destination = destination;
            _batchSize = batchSize;
        }

        public void Start(IJobContext jobContext)
        {
            IEnumerable<T> currentBatch;
            do
            {
                currentBatch = _source.GetBatchOfObjects<T>(_batchSize,
                    jobContext.CurrentRecord,
                    jobContext.MinimumModificationDate);
                var tasks = currentBatch.Select(obj => Task.Factory.StartNew(arg => _destination.MaybeUpdate<T>(obj), null));
                Task.WaitAll(tasks.ToArray());

                if (currentBatch.Any())
                {
                    jobContext.SetCurrentRecord(currentBatch.Last().Id);
                }

            } while (currentBatch.Count() >= _batchSize);
        }

      
    }
}