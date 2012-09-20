using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncErrorTracker : ISyncErrorTracker
    {
        public void TrackError(SyncEventArgs args, SessionContext sessionContext)
        {
            using(var db = new AftDbContext())
            {
                var context = db.SessionContexts.Find(sessionContext.Id);
                var syncEvent = new SyncEvent
                {
                    Destination = args.Destination.GetType().Name,
                    Error = args.Error == null ? null : args.Error.ToString(),
                    EventType = args.EventType,
                    ObjectType = args.ObjectType, 
                    SalsaKey = args.SalsaKey,
                    Data = args.Data,
                    SessionContext = context
                };
                db.SyncEvents.Add(syncEvent);
                db.SaveChanges();
            }
        }

      
    }
}
