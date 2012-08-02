using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.utilities
{
    class TestUtils
    {
        public static void Remove<T>(Expression<Func<T, bool>> expression) where T : class
        {
            using (var db = new AftDbContext())
            {
                db.Records<T>().Where(expression).ToList().ForEach(s => db.Records<T>().Remove(s));
                db.SaveChanges();
            }
        }

        public static void RemoveAll<T>() where T : class
        {
            using (var db = new AftDbContext())
            {
                db.Records<T>().ToList().ForEach(s => db.Records<T>().Remove(s));
                db.SaveChanges();
            }
        }
    }
}
