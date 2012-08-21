using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class LocalRepository : ISyncObjectRepository
    {
       
        public IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime minimumModifiedDate) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                DbQuery<T> queryable = db.Set<T>();
                queryable = IncludeRelations(queryable);
                return queryable.Where(s => s.ModifiedDate >= minimumModifiedDate && s.Id > startKey).OrderBy(s => s.Id).Take(batchSize).ToList();
            }
        }

        public int Add<T>(T syncObject) where T : class, ISyncObject
        {
            int id;
            using (var db = new AftDbContext())
            {
                db.Set<T>().Add(syncObject);
                db.SaveChanges();
                id = syncObject.Id;
            }
            NotifySyncEvent(this, new SyncEventArgs{Destination = this, EventType = SyncEventType.Add, SyncObject = syncObject});
            return id;
        }

        public void Update<T>(T syncObject) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                db.Set<T>().Attach(syncObject);
                db.Entry(syncObject).State = EntityState.Modified;
                db.SaveChanges();
            }
            NotifySyncEvent(this, new SyncEventArgs { Destination = this, EventType = SyncEventType.Update, SyncObject = syncObject });
        }

        public T GetByExternalKey<T>(int key) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                DbQuery<T> query = db.Set<T>();
                query = IncludeRelations(query);
                return query.SingleOrDefault(s => s.ExternalId == key);
            }
        }

        public T Get<T>(int key) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                DbQuery<T> query = db.Set<T>();
                query = IncludeRelations(query);

                return query.SingleOrDefault(s => s.Id == key);
            }
        }

        private static DbQuery<T> IncludeRelations<T>(DbQuery<T> query) where T : class, ISyncObject
        {
            var attribute = typeof (T).GetCustomAttributes(false).ToList()
                                .FirstOrDefault(a => a is IncludeRelationAttribute) as IncludeRelationAttribute;
            if (attribute != null)
                attribute.Relations.ForEach(r => query = query.Include(r));
            return query;
        }

        public DateTime CurrentTime
        {
            get
            {
                using (var db = new AftDbContext())
                {
                    return db.Database.SqlQuery<DateTime>("select GETDATE()").First();
                }
            }

        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };
    }
}
