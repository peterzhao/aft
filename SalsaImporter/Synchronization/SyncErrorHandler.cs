using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using SalsaImporter.Exceptions;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorHandler : ISyncErrorHandler
    {
        private readonly ConcurrentDictionary<ISyncObject, Exception> _pullingFailure
            = new ConcurrentDictionary<ISyncObject, Exception>();
        private readonly ConcurrentDictionary<string, string> _deletionFailure
           = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<ISyncObject, Exception> _pushingFailure
           = new ConcurrentDictionary<ISyncObject, Exception>();
        private readonly int _abortPullingThreshold;
        private readonly int _abortDeletionThreshold;
        private readonly int _abortPushingThreshold;

        public SyncErrorHandler(int abortPullingThreshold, int abortPushingThreshold, int abortDeletionThreshold)
        {
            _abortPullingThreshold = abortPullingThreshold;
            _abortPushingThreshold = abortPushingThreshold;
            _abortDeletionThreshold = abortDeletionThreshold;
        }

        public ConcurrentDictionary<ISyncObject, Exception> PullingFailure
        {
            get { return _pullingFailure; }
        }

        public ConcurrentDictionary<ISyncObject, Exception> PushingFailure
        {
            get { return _pushingFailure; }
        }

        public ConcurrentDictionary<string, string> DeletionFailure
        {
            get { return _deletionFailure; }
        }

        public void HandlePullObjectFailure(ISyncObject obj, Exception ex)
        {
            PullingFailure[obj] = ex;
            Logger.Error(String.Format("Failed to pull object:" + obj, ex));
            if (_abortPullingThreshold < PullingFailure.Keys.Count)
            {
                string message = "Failure to pull objects exceeded the threshold. Process aborted. Threshold:" + _abortPullingThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }
            
        }

        public void HandlePushObjectFailure(ISyncObject obj, Exception ex)
        {

            PushingFailure[obj] = ex;
            Logger.Error(String.Format("Failed to push object:" + obj, ex));
            if (_abortPushingThreshold < PushingFailure.Keys.Count)
            {
                string message = "Failure to push objects exceeded the threshold. Process aborted. Threshold:" + _abortPushingThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }

        }


        public void HandleDeleteObjectFailure(string suppoertKey)
        {
            DeletionFailure[suppoertKey] = suppoertKey;
            Logger.Error(String.Format("Failed to delete supporter(key:{0})", suppoertKey));
            if (_abortDeletionThreshold < DeletionFailure.Keys.Count)
            {
                string message = "Failure to delete supporters exceeded the threshold. Process aborted. Threshold:" + _abortDeletionThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }

        }

      
        public static TResult Try<TResult, TException>(Func<TResult> func, int tryTimes) where TException : Exception
        {
            int count = 0;
            while (true)
            {
                try
                {
                    return func();
                }
                catch (TException exception)
                {
                    string exceptionName = exception.GetType().Name;
                    count += 1;
                    if (count >= tryTimes)
                    {
                        string message = String.Format("Rethrow {0} after try {1} times. {2} ", exceptionName, tryTimes, exception.Message);
                        Logger.Error(message);
                        throw new ApplicationException(message);
                    }else
                    {
                        Logger.Warn(String.Format("Catched {0} and try again. Error:{1}", exceptionName, exception.Message));
                    }
                }
            }
        }
    }
}
