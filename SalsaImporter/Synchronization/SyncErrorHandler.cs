using System;
using System.Collections.Concurrent;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorHandler : ISyncErrorHandler
    {
        private readonly ConcurrentDictionary<ISyncObject, Exception> _failures
            = new ConcurrentDictionary<ISyncObject, Exception>();
        private readonly int _abortThreshold;

        public SyncErrorHandler(int abortThreshold)
        {
            _abortThreshold = abortThreshold;
        }

        public ConcurrentDictionary<ISyncObject, Exception> Failures
        {
            get { return _failures; }
        }

    
        public void HandleSyncObjectFailure(ISyncObject obj, ISyncObjectRepository destination, Exception ex)
        {
            Failures[obj] = ex;
            Logger.Error(String.Format("Failed to sync object:" + obj), ex);
            NotifySyncEvent(this, new SyncEventArgs{Destination = destination, EventType = SyncEventType.Error, SyncObject = obj, Error = ex});
            if (_abortThreshold < Failures.Keys.Count)
            {
                string message = "Sync failures exceeded the threshold. Process aborted. Threshold:" + _abortThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }
            
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate{};

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
                        string message = String.Format("Rethrow {0} after try {1} times. Error: {2} ", exceptionName, tryTimes, exception.Message);
                        Logger.Error(message, exception);
                        throw new ApplicationException(message);
                    }
                    else
                    {
                        string message = String.Format("Caught {0} on attempt {1}. Trying again. Error: {2}", exceptionName, count, exception.Message);
                        Logger.Warn(message, exception);
                    }
                }
            }
        }
    }
}
