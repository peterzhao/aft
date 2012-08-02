using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Aft;

namespace SalsaImporterTests
{
    [TestFixture]
    public class AftDbContextTests
    {
        private AftDbContext db;
        [SetUp]
        public void SetUp()
        {
            db = new AftDbContext();
        }
        [Test]
        public void ShouldCreateSupporterInAftDb()
        {
            var name = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter = new Supporter { Email = name + "@abc.com", First_Name = name, Last_Name = "UnitTest"};
            db.Supporters.Add(supporter);
            db.SaveChanges();
            Assert.Greater(supporter.Id, 0);
        }

        [Test]
        public void ShouldCreateSyncRunInAftDb()
        {
            var syncrun = new SyncRun { Complete = false, CurrentRecord = 0, LastUpdatedMinimum = DateTime.MinValue, StartTime = DateTime.Now};
            db.SyncRuns.Add(syncrun);
            db.SaveChanges();
            Assert.Greater(syncrun.Id, 0);
        }

        [TearDown]
        public void TearDown()
        {
            var supporters = db.Supporters.Where(s => s.Last_Name == "UnitTest");

            supporters.ToList().ForEach(s => db.Supporters.Remove(s));
            db.SaveChanges();
            db.Dispose();
        }
    }
}
