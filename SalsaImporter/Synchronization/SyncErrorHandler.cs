using System;
using System.Collections.Concurrent;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorHandler : ISyncErrorHandler
    {
        private readonly ConcurrentDictionary<object, Exception> _failures
            = new ConcurrentDictionary<object, Exception>();
        private readonly int _abortThreshold;

        public SyncErrorHandler(int abortThreshold)
        {
            _abortThreshold = abortThreshold;
        }

        public ConcurrentDictionary<object, Exception> Failures
        {
            get { return _failures; }
        }

    
        public void HandleSyncObjectFailure(SyncObject obj, object destination, Exception ex)
        {
            Logger.Error(String.Format("Failed to sync object: {0}", obj), ex);
            var syncEventArgs = new SyncEventArgs { SyncObject = obj, Destination = destination, EventType = SyncEventType.Error, Error = ex };
            HandleFailure(obj, ex, syncEventArgs);
        }

        public void HandleMappingFailure(string objectType, XElement obj, object source, Exception ex)
        {
            Logger.Error(String.Format("Failed to map object: {0}", obj), ex);
            var syncEventArgs = new SyncEventArgs
                                    {
                                        Destination = source,
                                        EventType = SyncEventType.Error,
                                        Error = ex,
                                        Data = obj.ToString(),
                                        ObjectType = objectType
                                    };
            HandleFailure(obj, ex, syncEventArgs);
        }

        private void HandleFailure(object obj, Exception ex, SyncEventArgs syncEventArgs)
        {
            Failures[obj] = ex;
            NotifySyncEvent(this, syncEventArgs);
            if (_abortThreshold < Failures.Keys.Count)
            {
                string message = "Sync failures exceeded the threshold. Process aborted. Threshold:" + _abortThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }
        }


        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate{};
    }
 }