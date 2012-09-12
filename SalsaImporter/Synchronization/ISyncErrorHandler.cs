using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandleSyncObjectFailure(SyncObject obj, object destination, Exception ex);
        void HandleMappingFailure(string objectType, XElement obj, object source, Exception ex);
        void HandleSalsaClientException(string objectType, int salsaKey, object destination, Exception ex);
        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}