using System;
using System.Collections.Generic;
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
        public void SycTests()
        {
            Config.Environment = Config.PerformanceTest;
        }

        private void GetSupportersFromAft(AftDbContext db, int batchSize, int? totalLimit, Action<List<Supporter>> batchHandler)
        {
            int start = 0;
            int total = 0;
            int batchCount = 0;
            while (true)
            {
                var supporters = db.Supporters.OrderBy(s => s.Id).Where(s => s.Id > start && s.supporter_KEY == null).Take(batchSize).ToList();
                if (supporters.Count == 0) return;
                Logger.Info(String.Format("Pulling supporter from aft... batch:{0} start: {1} Get: {2}", batchCount, supporters.First().Id, supporters.Count));
                start = supporters.Last().Id;
                batchHandler(supporters);
                if (supporters.Count < batchSize) return;
                total += supporters.Count;
                if (totalLimit != null && total >= totalLimit.Value) return;
                batchCount += 1;
            }
        }


        [Test]
        public void ShouldPushSupportersToSalsa()
        {
            Logger.Info("Start performance test...");
            var salsa = new SalsaClient();
            salsa.Authenticate();
            var begin = DateTime.Now;
            var db = new AftDbContext();
            var mapper = new SupporterMapper();
            var batchSize = 100;
            int? totalLimit = 1000;
            GetSupportersFromAft(db, batchSize, totalLimit, supporters =>
                                {
                                    var nameValuesList = supporters.Select(mapper.ToNameValues).ToList();
                                    salsa.SaveSupporters(nameValuesList);
                                    nameValuesList.ForEach(nameValues =>
                                    {
                                        var supporter = supporters.Find(s => s.Id == int.Parse(nameValues["uid"]));
                                        supporter.supporter_KEY = int.Parse(nameValues["supporter_KEY"]);
                                    });
                                    Logger.Debug("update aft db for supporter_key.");
                                    db.SaveChanges();
                                });

            var finished = DateTime.Now;
            Logger.Info("finished:" + (finished - begin).TotalSeconds);
        }


    }
}