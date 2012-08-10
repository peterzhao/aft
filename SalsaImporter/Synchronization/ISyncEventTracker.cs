using System;
using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public interface ISyncEventTracker
    {
        void TrackEvent(SyncEventArgs args);
        void EachyncEventsForSession(SessionContext sessionContext, Action<IQueryable<SyncEvent>> action);
    }
}