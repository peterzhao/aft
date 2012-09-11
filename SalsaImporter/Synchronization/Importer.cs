using System;
using System.Linq;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class Importer :ISyncJob 
    {
        private readonly ISalsaRepository _source;
        private readonly IQueueRepository _destination;
        private readonly ISyncErrorHandler _errorHandler;
        private readonly int _batchSize;
        private readonly string _objectType;
        private readonly string _queueName;

        public string Name { get; set; }


        public Importer(ISalsaRepository source, IQueueRepository destination, ISyncErrorHandler errorHandler, int batchSize, string name, string objectType, string queueName)
        {
            Name = name;
            _objectType = objectType;
            _queueName = queueName;
            _source = source;
            _destination = destination;
            _errorHandler = errorHandler;
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
                currentBatch.ForEach(Enqueue);
                if (currentBatch.Any())
                    jobContext.SetCurrentRecord(currentBatch.Last().SalsaKey);
                else
                    break;                    
            };
        }

        private void Enqueue(SyncObject obj)
        {
            try
            {
                _destination.Enqueue(_queueName, obj);
            }
            catch(Exception ex)
            {
                _errorHandler.HandleSyncObjectFailure(obj, this, ex);
            }
        }
    }
}