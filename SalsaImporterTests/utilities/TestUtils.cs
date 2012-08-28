using System;
using System.Linq;
using System.Linq.Expressions;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Utilities
{
    class TestUtils
    {

        private static SalsaClient SalsaClient
        {
            get { return new SalsaClient(); }
        }

        public static SalsaRepository SalsaRepository
        {
            get { return new SalsaRepository(SalsaClient, new MapperFactory(), new SyncErrorHandler(10)); }
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
                db.Database.ExecuteSqlCommand("delete  from SyncEvents");
                db.Database.ExecuteSqlCommand("delete  from JobContexts");
                db.Database.ExecuteSqlCommand("delete  from SessionContexts");
            }
        }

        public static void ClearAllQueues()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete from Supporter_SalsaToAftQueue");
            }
        }


         }
}
