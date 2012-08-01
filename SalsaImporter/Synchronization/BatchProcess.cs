using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class BatchProcess
    {
        private ISyncObjectRepository _localRepository;
        private readonly ISyncObjectRepository _externalRepository;
        private readonly ISyncLog _syncLog;
        private readonly IObjectProcess _objectProcess;

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
            DateTime start = DateTime.Now; //todo: = SyncObject.StartPullTime reset when status is finished
            IEnumerable<T> objects = null;
            int startKey = _syncLog.LastPulledKey;
            var lastProcessedDatetime = _syncLog.LastPullDateTime;
            do
            {
                objects = _externalRepository.GetBatchOfObjects<T>(batchSize, startKey, lastProcessedDatetime);
                var tasks = objects.Select(obj => Task.Factory.StartNew(arg => _objectProcess.ProcessPulledObject<T>(obj), null));
                Task.WaitAll(tasks.ToArray());
                if (!objects.Any())
                {
                   _syncLog.PullingCompleted(); 
                   break;
                }
                startKey = objects.Last().ExternalKey.Value;
                _syncLog.LastPulledKey = startKey;

            } while (true);
        }

        public void PushToExternal<T>(int batchSize) where T : ISyncObject
        {
            DateTime start = DateTime.Now; //todo: = SyncObject.StartPullTime reset when status is finished
            IEnumerable<T> objects = null;
            int startKey = _syncLog.LastPulledKey;
            var lastProcessedDatetime = _syncLog.LastPullDateTime;
            do
            {
                objects = _externalRepository.GetBatchOfObjects<T>(batchSize, startKey, lastProcessedDatetime);
                var tasks = objects.Select(obj => Task.Factory.StartNew(arg => _objectProcess.ProcessPulledObject<T>(obj), null));
                Task.WaitAll(tasks.ToArray());
                if (!objects.Any())
                {
                    _syncLog.PullingCompleted();
                    break;
                }
                startKey = objects.Last().ExternalKey.Value;
                _syncLog.LastPulledKey = startKey;

            } while (true);
        }
    }
}