using System;
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
        private static void SaveSupportersToSalsa(AftDbContext db, SalsaClient salsa, int start, int blockSize)
        {
            var mapper = new SupporterMapper();
            var supporters = db.Supporters.OrderBy(s => s.Id).Skip(start).Take(blockSize).ToList()
                .Select(mapper.ToNameValues).ToList();
            salsa.SaveSupporters(supporters);
            supporters.ForEach(s =>
                                   {
                                       var supporter = db.Supporters.Find(int.Parse(s["uid"]));
                                       supporter.supporter_KEY = int.Parse(s["supporter_KEY"]);
                                   });
            Logger.Debug("update aft db for supporter_key.");
            db.SaveChanges();
        }


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
            var blockSize = 100;
            for (var i = 0; i < total; i += blockSize)
            {
                Console.WriteLine(String.Format("batch {0} starting...", i));
                SaveSupportersToSalsa(db, salsa, i, blockSize);
            }

            var finished = DateTime.Now;
            Console.WriteLine("finished:" + (finished - begin).TotalSeconds);
        }
    }
}