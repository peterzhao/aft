using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests.PerformanceTest
{
    [TestFixture]
    [Category("PerformanceTest")]
    public class SyncTests
    {
        private static void SaveSupportersToSalsa(SalsaClient salsa, int start, int blockSize)
        {
            var db = new AftDbContext();
            var mapper = new SupporterMapper();
            List<NameValueCollection> supporters =
                db.Supporters.OrderBy(s => s.Id).Skip(start).Select(mapper.ToNameValues).Take(blockSize).ToList();
            salsa.SaveSupporters(supporters);
            supporters.ForEach(s =>
                                   {
                                       int uid = int.Parse(s["uid"]);
                                       Console.WriteLine("uid:" + uid);
                                       Supporter supporter = db.Supporters.Find(uid);
                                       string supporterKey = s["supporter_KEY"];
                                       Console.WriteLine("SupportKey:" + supporterKey);
                                       supporter.supporter_KEY = int.Parse(supporterKey);
                                   });

            db.SaveChanges();
        }

        [Test]
        public void ShouldPushSupportersToSalsa()
        {
            DateTime begin = DateTime.Now;
            var salsa = new SalsaClient();
            salsa.Authenticate();
            salsa.DeleteAllSupporters();
            var db = new AftDbContext();
            int total = db.Supporters.Count();
            total = 1000;
            int blockSize = 100;
            for (int i = 0; i < total; i += blockSize)
            {
                Console.WriteLine(String.Format("batch {0} starting...", i));
                SaveSupportersToSalsa(salsa, i, blockSize);
            }

            db.Dispose();
            DateTime finished = DateTime.Now;
            Console.WriteLine("finished:" + (finished - begin).TotalSeconds);
        }
    }
}