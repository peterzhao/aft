using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class Exporter:ISyncJob
    {
        private readonly ISalsaRepository _destination;
        private readonly ISyncErrorHandler _errorHandler;
        private readonly IQueueRepository _source;
        private readonly int _batchSize;
        private readonly string _objectType;
        private readonly string _queueName;
        public string Name { get;  set; }

        public Exporter(IQueueRepository source, ISalsaRepository destination, ISyncErrorHandler errorHandler, int batchSize, string name, string objectType, string queueName)
        {
            Name = name;
            _objectType = objectType;
            _queueName = queueName;
            _destination = destination;
            _errorHandler = errorHandler;
            _source = source;
            _batchSize = batchSize;
        }

        public string ObjectType { get { return _objectType; } }


        public void Start(IJobContext jobContext)
        {
            for (int batchCount = 1; ; batchCount++) 
            {
                if(!Config.SalsaWritable) throw new AccessViolationException(String.Format("Cannot export objects to Salsa. {0} is in read-only mode.", Config.Environment));
                Logger.Debug("Dequeue in batch " + batchCount + " with batch size:" + _batchSize + " " + Name);
                var currentBatch = _source.GetBatchOfObjects(_objectType,
                                                         _queueName,
                                                         _batchSize,
                                                         jobContext.CurrentRecord).ToList();
                var tasks = currentBatch.Select(syncObject => Task.Factory.StartNew(arg => ExportSyncObject(syncObject, jobContext), null));
                Task.WaitAll(tasks.ToArray());
                Logger.Trace("Updating current record...");
                if (currentBatch.Any())
                    jobContext.SetCurrentRecord(currentBatch.Last().QueueId);
                else
                    break;                    
            };
        }

        private void ExportSyncObject(SyncObject syncObject, IJobContext jobContext)
        {
            try
            {
                if (_destination.Save(syncObject))
                {
                    _source.UpdateStatus(_queueName, syncObject.QueueId, QueueRepository.QueueStatusExported, DateTime.Now);
                    jobContext.CountSuccess();
                }
                else
                {
                    _source.UpdateStatus(_queueName, syncObject.QueueId, QueueRepository.QueueStatusSkipped, DateTime.Now);
                    jobContext.CountIdenticalObject();
                }
                _source.Dequeue(_queueName, syncObject.QueueId);

            }           
            catch (SaveToSalsaException ex)
            {

                _errorHandler.HandleSyncObjectFailure(syncObject, this, ex);
                _source.UpdateStatus(_queueName, syncObject.QueueId, QueueRepository.QueueStatusError);
            }
        }
    }
}
