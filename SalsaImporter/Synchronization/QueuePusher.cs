using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class QueuePusher :ISyncJob 
    {
        private readonly ISyncObjectRepository _source;
        private readonly QueueRepository _destination;
        private readonly int _batchSize;
        private readonly string _objectType;

        public string Name { get; set; }


        public QueuePusher(ISyncObjectRepository source, QueueRepository destination, int batchSize, string name, string objectType)
        {
            Name = name;
            _objectType = objectType;
            _source = source;
            _destination = destination;
            _batchSize = batchSize;
        }

        public void Start(IJobContext jobContext)
        {
            IEnumerable<SyncObject> currentBatch;
            int batchCount = 0;
            do
            {
                batchCount += 1;
                Logger.Debug("Running batch " + batchCount + " with batch size:" + _batchSize + " " + Name);
                currentBatch = _source.GetBatchOfObjects(_objectType,
                    _batchSize,
                    jobContext.CurrentRecord,
                    jobContext.MinimumModificationDate).ToList();

                foreach (var obj in currentBatch)
                {
                    _destination.Push(obj);
                }
                
                if (currentBatch.Any())
                    jobContext.SetCurrentRecord(currentBatch.Last().Id);
                
            } while (currentBatch.Count() >= _batchSize);
        }

      
    }
}