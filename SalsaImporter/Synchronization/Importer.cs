using System.Linq;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class Importer :ISyncJob 
    {
        private readonly ISalsaRepository _source;
        private readonly IQueueRepository _destination;
        private readonly int _batchSize;
        private readonly string _objectType;
        private readonly string _queueName;

        public string Name { get; set; }


        public Importer(ISalsaRepository source, IQueueRepository destination, int batchSize, string name, string objectType, string queueName)
        {
            Name = name;
            _objectType = objectType;
            _queueName = queueName;
            _source = source;
            _destination = destination;
            _batchSize = batchSize;
        }

        public string ObjectType { get { return _objectType; } }

        public void Start(IJobContext jobContext)
        {
            for (int batchCount = 1; ; batchCount++) 
            {
                Logger.Debug("Running batch " + batchCount + " with batch size:" + _batchSize + " " + Name);
                var currentBatch = _source.GetBatchOfObjects(_objectType,
                                                         _batchSize,
                                                         jobContext.CurrentRecord,
                                                         jobContext.MinimumModificationDate).ToList();
                currentBatch.ForEach(obj => _destination.Push(obj, _queueName));
                if (currentBatch.Any())
                    jobContext.SetCurrentRecord(currentBatch.Last().SalsaKey);
                else
                    break;                    
            };
        }

      
    }
}