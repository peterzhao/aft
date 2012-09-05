using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class Exporter:ISyncJob
    {
        private readonly ISalsaRepository _destination;
        private readonly IQueueRepository _source;
        private readonly int _batchSize;
        private readonly string _objectType;
        private readonly string _queueName;
        public string Name { get;  set; }

        public Exporter(IQueueRepository source, ISalsaRepository destination, int batchSize, string name, string objectType, string queueName)
        {
            Name = name;
            _objectType = objectType;
            _queueName = queueName;
            _destination = destination;
            _source = source;
            _batchSize = batchSize;
        }

        public string ObjectType { get { return _objectType; } }


        public void Start(IJobContext jobContext)
        {
            for (int batchCount = 1; ; batchCount++) 
            {
                Logger.Debug("Dequeue in batch " + batchCount + " with batch size:" + _batchSize + " " + Name);
                var currentBatch = _source.DequeueBatchOfObjects(_objectType,
                                                          _queueName,
                                                         _batchSize,
                                                         jobContext.CurrentRecord).ToList();
                var tasks = currentBatch.Select(syncObject => Task.Factory.StartNew(arg => _destination.Save(syncObject), null));
                Task.WaitAll(tasks.ToArray());
                if (currentBatch.Any())
                    jobContext.SetCurrentRecord(currentBatch.Last().QueueId);
                else
                    break;                    
            };
        }
    }
}
