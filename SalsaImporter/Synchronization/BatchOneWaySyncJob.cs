using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class BatchOneWaySyncJob<T> :ISyncJob where T: class , ISyncObject, new()
    {
        private readonly ISyncObjectRepository _source;
        private readonly IObjectUpdater _destination;
        private int _batchSize;

        public string Name { get; set; }


        public BatchOneWaySyncJob(ISyncObjectRepository source, IObjectUpdater destination, int batchSize, string name)
        {
            Name = name;
            _source = source;
            _destination = destination;
            _batchSize = batchSize;
        }

        public void Start(IJobContext jobContext)
        {
            IEnumerable<T> currentBatch;
            int batchCount = 0;
            do
            {
                batchCount += 1;
                Logger.Debug("Running batch " + batchCount + " with batch size:" + _batchSize + " " + Name);
                currentBatch = _source.GetBatchOfObjects<T>(_batchSize,
                    jobContext.CurrentRecord,
                    jobContext.MinimumModificationDate);
                var tasks = currentBatch.Select(obj => Task.Factory.StartNew(arg => _destination.Update<T>(obj), null));
                Task.WaitAll(tasks.ToArray());

                if (currentBatch.Any())
                    jobContext.SetCurrentRecord(currentBatch.Last().Id);
                
            } while (currentBatch.Count() >= _batchSize);
        }

      
    }
}