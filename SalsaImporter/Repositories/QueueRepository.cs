using System;
using System.Collections.Generic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly AftDbContext _db;
        
        public QueueRepository(AftDbContext db)
        {
            _db = db;
        }

        public void Push(SyncObject syncObject, string tableName)
        {
            List<string> fields = new List<string> { "First_Name", "Last_Name", "Email" };

            _db.InsertToQueue(syncObject, tableName, fields);

            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Add, Destination = this, SyncObject = syncObject });
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };
    }
}
