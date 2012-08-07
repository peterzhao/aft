using System;
using System.Collections.Concurrent;
using SalsaImporter.Exceptions;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorHandler : ISyncErrorHandler
    {
        private readonly ConcurrentDictionary<ISyncObject, Exception> _addFailure
            = new ConcurrentDictionary<ISyncObject, Exception>();
        private readonly ConcurrentDictionary<string, string> _deleteFailure
           = new ConcurrentDictionary<string, string>();
        private readonly int _abortAddThreshold;
        private readonly int _abortDeletionThreshold;

        public SyncErrorHandler(int abortAddThreshold, int abortDeletionThreshold)
        {
            _abortAddThreshold = abortAddThreshold;
            _abortDeletionThreshold = abortDeletionThreshold;
        }

        public ConcurrentDictionary<ISyncObject, Exception> AddFailure
        {
            get { return _addFailure; }
        }

        public ConcurrentDictionary<string, string> DeleteFailure
        {
            get { return _deleteFailure; }
        }

        public void HandleAddObjectFailure(ISyncObject obj, Exception ex)
        {
            AddFailure[obj] = ex;
            Logger.Error(String.Format("Failed to add object:" + obj), ex);
            if (_abortAddThreshold < AddFailure.Keys.Count)
            {
                string message = "Add failures exceeded the threshold. Process aborted. Threshold:" + _abortAddThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }
            
        }

        public void HandleDeleteObjectFailure(string suppoertKey)
        {
            DeleteFailure[suppoertKey] = suppoertKey;
            Logger.Error(String.Format("Failed to delete supporter(key:{0})", suppoertKey));
            if (_abortDeletionThreshold < DeleteFailure.Keys.Count)
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
                        Logger.Warn(String.Format("Caught {0} and try again. Error:{1}", exceptionName, exception.Message));
                    }
                }
            }
        }
    }
}
