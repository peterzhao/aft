using System;
using System.Collections.Concurrent;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorHandler : ISyncErrorHandler
    {
        private int _errorCount;
        private readonly int _abortThreshold;

        public SyncErrorHandler(int abortThreshold)
        {
            _abortThreshold = abortThreshold;
        }

      

    
        public void HandleSyncObjectFailure(SyncObject obj, object destination, Exception ex)
        {
            Logger.Error(String.Format("Failed to sync object: {0}", obj), ex);
            var syncEventArgs = new SyncEventArgs { SyncObject = obj, Destination = destination, EventType = SyncEventType.Error, Error = ex };
            HandleFailure(syncEventArgs);
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
            HandleFailure(syncEventArgs);
        }

      

        public void HandleSalsaClientException(string objectType, int salsaKey, object destination, Exception ex)
        {
            Logger.Error(String.Format("Got Salsa client error for: {0} with key:{1}", objectType, salsaKey), ex);
            var syncEventArgs = new SyncEventArgs
            {
                Destination = destination,
                EventType = SyncEventType.Error,
                Error = ex,
                SalsaKey = salsaKey,
                ObjectType = objectType
            };
            HandleFailure(syncEventArgs);
        }

        private void HandleFailure(SyncEventArgs syncEventArgs)
        {
            lock (this) _errorCount += 1;

            NotifySyncEvent(this, syncEventArgs);
            if (_abortThreshold < _errorCount)
            {
                string message = "Sync failures exceeded the threshold. Process aborted. Threshold:" + _abortThreshold;
                Logger.Fatal(message);
                throw new SyncAbortedException(message);
            }
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate{};
    }
 }