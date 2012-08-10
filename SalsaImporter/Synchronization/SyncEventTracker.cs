using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncEventTracker : ISyncEventTracker
    {
        public void TrackEvent(SyncEventArgs args)
        {
            var syncEvent = new SyncEvent
                                {
                                    Data = args.SyncObject.ToString(),
                                    Destination = args.Destination.GetType().Name,
                                    Error = args.Error == null? null : args.Error.ToString(),
                                    ObjectId = args.SyncObject.Id,
                                    ExternalId = args.SyncObject.ExternalId.HasValue ? args.SyncObject.ExternalId.Value : 0,
                                    EventType =args.EventType,
                                    ObjectType = args.SyncObject.GetType().Name,
                                    SessionContext = SyncSession.CurrentSession().CurrentContext,

                                };
            using(var db = new AftDbContext())
            {
                db.SyncEvents.Add(syncEvent);
                db.SaveChanges();
            }
        }

        public void EachyncEventsForSession(SessionContext sessionContext, Action<IQueryable<SyncEvent>> action )
        {
            using (var db = new AftDbContext())
            {
                action(db.SyncEvents.Where(ev => ev.SessionContext.Id == sessionContext.Id));
            }
        }
    }
}
