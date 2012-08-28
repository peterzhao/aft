using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly IAftDbContext _db;
        
        public QueueRepository(IAftDbContext db)
        {
            _db = db;
        }

        public void Push(SyncObject syncObject, string tableName)
        {
            _db.InsertToQueue(syncObject, tableName, syncObject.FieldNames);
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Add, Destination = this, SyncObject = syncObject });
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };
    }
}
