using System;
using System.Collections.Specialized;
using SalsaImporter.Aft;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandleSyncObjectFailure(ISyncObject obj, Exception ex);
    }
}