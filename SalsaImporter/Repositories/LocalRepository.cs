using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class LocalRepository : ISyncObjectRepository
    {
       
        public IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime lastProcessedDateTime) where T : ISyncObject
        {
            throw new NotImplementedException("GetBatchOfObjects");
        }

        public int Add<T>(T syncObject) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                db.Records<T>().Add(syncObject);
                db.SaveChanges();
                return syncObject.Id;
            }
        }

        public void Update<T>(T syncObject) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                db.Records<T>().Attach(syncObject);
                db.Entry(syncObject).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public T GetByExternalKey<T>(int key) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                return db.Records<T>().SingleOrDefault(s => s.ExternalId == key);
            }
        }

        public T Get<T>(int key) where T : class, ISyncObject
        {
            using (var db = new AftDbContext())
            {
                return db.Records<T>().SingleOrDefault(s => s.Id == key);
            }
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
    }
}
