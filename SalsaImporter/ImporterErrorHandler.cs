using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SalsaImporter
{
    public class ImporterErrorHandler
    {
        private readonly ConcurrentDictionary<string, NameValueCollection> _failedRecordsToCreate 
            = new ConcurrentDictionary<string, NameValueCollection>();
        private readonly ConcurrentDictionary<string, string> _failedRecordsToDelete
           = new ConcurrentDictionary<string, string>();
        private readonly int _abortCreateThreshold;
        private readonly int _abortDeleteThreshold;

        public ImporterErrorHandler(int abortCreateThreshold, int abortDeleteThreshold)
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


        public void HandleCreateObjectFailure(NameValueCollection data)
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
