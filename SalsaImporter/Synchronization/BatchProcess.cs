using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalsaImporter.Synchronization
{
    public class BatchProcess
    {
        private ISyncObjectRepository _localRepository;
        private ISyncObjectRepository _externalRepository;
        private ISyncLog _syncLog;
        private IObjectProcess _objectProcess;

        public BatchProcess(ISyncObjectRepository localRepository, 
            ISyncObjectRepository externalRepository,
            ISyncLog syncLog, IObjectProcess objectProcess)
        {
            _localRepository = localRepository;
            _externalRepository = externalRepository;
            _syncLog = syncLog;
            _objectProcess = objectProcess;
        }

        public void PullFromExternal<T>(int batchSize) where T:ISyncObject
        {
            IEnumerable<ISyncObject> objects = null;
            int startKey = _syncLog.LastPulledKey;
            var lastProcessedDatetime = _syncLog.LastPullDateTime;
            do
            {
                objects = _externalRepository.GetBatchOfObjects<T>(batchSize, startKey, lastProcessedDatetime);
                var tasks = objects.Select(obj => Task.Factory.StartNew(arg => _objectProcess.ProcessPulledObject(obj), null));
                Task.WaitAll(tasks.ToArray());
                if (objects.Any())
                    startKey = objects.Last().ExternalKey.Value;
                else
                {
                    _syncLog.LastPulledKey = 0;
                    _syncLog.LastPullDateTime = DateTime.Now;
                }
                _syncLog.LastPulledKey = startKey;
            } while (objects.Any());
        }
    }
}