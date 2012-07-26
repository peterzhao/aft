using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests.PerformanceTest
{
    [TestFixture]
    public class SyncTests
    {
        [Test]
        public void ShouldPushSupportersToSalsa()
        {
            var begin = DateTime.Now;
            var salsa = new SalsaClient();
            salsa.Authenticate();
            salsa.DeleteAllSupporters();
            var db = new AftDbContext();
            var total = db.Supporters.Count();
            total = 1000;
            int blockSize = 100;
            for (int i = 0; i < total; i += blockSize )
            {
                Console.WriteLine(String.Format("batch {0} starting...", i));
                SaveSupportersToSalsa(salsa, i, blockSize);

            }
           
            db.Dispose();
            var finished = DateTime.Now;
            Console.WriteLine("finished:" + (finished - begin).TotalSeconds);

        }

        private static void SaveSupportersToSalsa(SalsaClient salsa, int start, int blockSize)
        {
            var db = new AftDbContext();
            var mapper = new SupporterMapper();
            var supporters = db.Supporters.OrderBy(s => s.Id).Skip(start).Select(mapper.ToNameValues).Take(blockSize).ToList();
            salsa.SaveSupporters(supporters);
            supporters.ForEach(s =>
                                   {
                                       var uid = int.Parse(s["uid"]);
                                       Console.WriteLine("uid:" + uid);
                                       var supporter = db.Supporters.Find(uid);
                                       string supporterKey = s["supporter_KEY"];
                                       Console.WriteLine("SupportKey:" + supporterKey);
                                       supporter.supporter_KEY = int.Parse(supporterKey);
                                   });

            db.SaveChanges();
        }
    }
}
