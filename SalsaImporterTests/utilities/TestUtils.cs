using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.utilities
{
    class TestUtils
    {
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
            var salsaClient = new SalsaClient(new SyncErrorHandler(10, 10, 10));
            salsaClient.DeleteAllObjects(objectType, 100, true);
        }

        public static void CreateSalsa<T>(params T[] objects) where T : class, ISyncObject
        {
            var salsaRepository = new SalsaRepository(new SalsaClient(new SyncErrorHandler(10, 10, 10)), new MapperFactory());
            objects.ToList().ForEach(syncObject => syncObject.Id = salsaRepository.Add(syncObject));
        }

        public static void UpdateSalsa<T>(params T[] objects) where T : class, ISyncObject
        {
            var salsaRepository = new SalsaRepository(new SalsaClient(new SyncErrorHandler(10, 10, 10)), new MapperFactory());
            objects.ToList().ForEach(salsaRepository.Update);
        }
    
    }
}
