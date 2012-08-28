using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandleSyncObjectFailure(SyncObject obj, ISalsaRepository destination, Exception ex);
        void HandleMappingFailure(string objectType, XElement obj, ISalsaRepository source, Exception ex);
        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}