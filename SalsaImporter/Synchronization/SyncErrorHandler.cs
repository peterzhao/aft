using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorHandler : ISyncErrorHandler
    {
        private readonly ConcurrentDictionary<string, NameValueCollection> _failedRecordsToCreate 
            = new ConcurrentDictionary<string, NameValueCollection>();
        private readonly ConcurrentDictionary<string, string> _failedRecordsToDelete
           = new ConcurrentDictionary<string, string>();
        private readonly int _abortCreateThreshold;
        private readonly int _abortDeleteThreshold;

        public SyncErrorHandler(int abortCreateThreshold, int abortDeleteThreshold)
        {
            _abortCreateThreshold = abortCreateThreshold;
            _abortDeleteThreshold = abortDeleteThreshold;
        }

        public ConcurrentDictionary<string, NameValueCollection> FailedRecordsToCreate
        {
            get { return _failedRecordsToCreate; }
        }

        public ConcurrentDictionary<string, string> FailedRecordsToDelete
        {
            get { return _failedRecordsToDelete; }
        }


        public void HandlePushObjectFailure(NameValueCollection data)
        {
            _failedRecordsToCreate[data["uid"]] = data;
            Logger.Error(String.Format("Failed to create supporter(id:{0})", data["uid"]));
            if(_abortCreateThreshold < _failedRecordsToCreate.Keys.Count)
            {
                string message = "Failure to create supporters exceeded the threshold. Process aborted. Threshold:" + _abortCreateThreshold;
                Logger.Fatal(message);
                throw new OperationCanceledException(message);
            }

        }

        public void HandlePullObjectFailure(ISyncObject obj, Exception ex)
        {
            
        }

        public void HandleDeleteObjectFailure(string suppoertKey)
        {
            FailedRecordsToDelete[suppoertKey] = suppoertKey;
            Logger.Error(String.Format("Failed to delete supporter(key:{0})", suppoertKey));
            if (_abortDeleteThreshold < FailedRecordsToDelete.Keys.Count)
            {
                string message = "Failure to delete supporters exceeded the threshold. Process aborted. Threshold:" + _abortDeleteThreshold;
                Logger.Fatal(message);
                throw new OperationCanceledException(message);
            }

        }

        public void HandlePushObjectFailure(ISyncObject obj, Exception ex)
        {
            
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
