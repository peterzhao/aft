using System;
using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public interface ISyncEventTracker
    {
        void TrackEvent(SyncEventArgs args, SessionContext sessionContext);
        void SyncEventsForSession(SessionContext sessionContext, Action<IQueryable<SyncEvent>> action);
    }
}