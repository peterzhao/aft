using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandleSyncObjectFailure(ISyncObject obj, ISyncObjectRepository destination, Exception ex);
        void HandleMappingFailure(string objectType, XElement obj, ISyncObjectRepository source, Exception ex);
        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}