using System;
using System.Linq;
using System.Linq.Expressions;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Utilities
{
    class TestUtils
    {

        private static SalsaClient SalsaClient
        {
            get { return new SalsaClient(new SyncErrorHandler(10, 10)); }
        }

        public static SalsaRepository SalsaRepository
        {
            get { return new SalsaRepository(SalsaClient, new MapperFactory()); }
        }

        public static LocalRepository LocalRepository
        {
            get { return new LocalRepository(); }
        }

        public static void RemoveLocal<T>(Expression<Func<T, bool>> expression) where T : class
        {
            using (var db = new AftDbContext())
            {
                db.Records<T>().Where(expression).ToList().ForEach(s => db.Records<T>().Remove(s));
                db.SaveChanges();
            }
        }

        public static void RemoveAllLocal<T>() where T : class
        {
            using (var db = new AftDbContext())
            {
                db.Records<T>().ToList().ForEach(s => db.Records<T>().Remove(s));
                db.SaveChanges();
            }
        }

        public static void RemoveAllSalsa(string objectType)
        {
            SalsaClient.DeleteAllObjects(objectType, 100, true);
        }
 
        public static void CreateSalsa<T>(params T[] objects) where T : class, ISyncObject
        {
            objects.ToList().ForEach(syncObject => syncObject.Id = SalsaRepository.Add(syncObject));
        }

        public static void UpdateSalsa<T>(params T[] objects) where T : class, ISyncObject
        {
            objects.ToList().ForEach(SalsaRepository.Update);
        }

        public static void ClearAllSessions()
        {
            using(var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete  from JobContexts");
                db.Database.ExecuteSqlCommand("delete  from SessionContexts");
            }
        }

        public static void CreateLocal<T>(params T[] objects) where T : class, ISyncObject
        {
            objects.ToList().ForEach(syncObject => LocalRepository.Add(syncObject));
        }

        public static void UpdateLocal<T>(params T[] objects) where T : class, ISyncObject
        {
            objects.ToList().ForEach(LocalRepository.Update);
        }

        public static void RemoveAllSupporter()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("truncate table Supporters");
            }
        }
    }
}
