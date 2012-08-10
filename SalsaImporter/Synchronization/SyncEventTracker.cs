﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncEventTracker : ISyncEventTracker
    {
        public void TrackEvent(SyncEventArgs args, SessionContext sessionContext)
        {
            using(var db = new AftDbContext())
            {
                var context = db.SessionContexts.Find(sessionContext.Id);
                var syncEvent = new SyncEvent
                {
                    Data = args.SyncObject.ToString(),
                    Destination = args.Destination.GetType().Name,
                    Error = args.Error == null ? null : args.Error.ToString(),
                    ObjectId = args.SyncObject.Id,
                    ExternalId = args.SyncObject.ExternalId.HasValue ? args.SyncObject.ExternalId.Value : 0,
                    EventType = args.EventType,
                    ObjectType = args.SyncObject.GetType().Name,
                    SessionContext = context,

                };
                db.SyncEvents.Add(syncEvent);
                db.SaveChanges();
            }
        }

        public void SyncEventsForSession(SessionContext sessionContext, Action<IQueryable<SyncEvent>> action )
        {
            using (var db = new AftDbContext())
            {
                action(db.SyncEvents.Where(ev => ev.SessionContext.Id == sessionContext.Id));
            }
        }
    }
}