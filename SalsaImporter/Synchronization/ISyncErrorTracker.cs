using System;
using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorTracker
    {
        void TrackError(SyncEventArgs args, SessionContext sessionContext);
    }
}