using System;
using System.Collections.Specialized;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandleSyncObjectFailure(ISyncObject obj, ISyncObjectRepository destination, Exception ex);
        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}