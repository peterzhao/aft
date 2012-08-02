using System;
using System.Linq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncLogTests
    {
        private AftDbContext db;
        [SetUp]
        public void SetUp()
        {
            db = new AftDbContext();
            db.SyncRuns.ToList().ForEach(s => db.SyncRuns.Remove(s));
            db.SaveChanges();
        }

        [Test]
        public void ShouldGiveMinimumTimeWhenNoPriorSynchronizations()
        {
            var syncLog = new SyncLog(db, DateTime.Now);
            Assert.AreEqual(DateTime.MinValue, syncLog.LastPullDateTime);
        }

        [Test]
        public void ShouldGiveLastStartTimeWhenPriorCompletedSynchronization()
        {
            DateTime firstStartTime = DateTime.Now;
            var firstSyncLog = new SyncLog(db, firstStartTime);
            
            firstSyncLog.PullingCompleted();

            var secondSyncLog = new SyncLog(db, DateTime.Now);

            Assert.AreEqual(firstStartTime, secondSyncLog.LastPullDateTime);
        }

        [Test]
        public void ShouldGiveLastKeyZeroAsStartKeyWhenNoPriorSynchronizations()
        {
            var syncLog = new SyncLog(db, DateTime.Now);
            Assert.AreEqual(0, syncLog.LastPulledKey);
        }

        [Test]
        public void ShouldRetainLastKey()
        {
            var syncLog = new SyncLog(db, DateTime.Now);
            syncLog.LastPulledKey = 100;
            Assert.AreEqual(100, syncLog.LastPulledKey);
        }


        [Test]
        public void ShouldResumeToLastKey()
        {
            var firstSyncLog = new SyncLog(db, DateTime.Now);
            firstSyncLog.LastPulledKey = 100;
            db.SaveChanges();

            var secondSyncLog = new SyncLog(db, DateTime.Now);
            Assert.AreEqual(100, secondSyncLog.LastPulledKey);
        }

        [Test]
        public void ShouldStartAtKeyZeroWhenPriorRunComplete()
        {
            var firstSyncLog = new SyncLog(db, DateTime.Now);
            firstSyncLog.LastPulledKey = 100;
            firstSyncLog.PullingCompleted();

            var secondSyncLog = new SyncLog(db, DateTime.Now);
            Assert.AreEqual(0, secondSyncLog.LastPulledKey);
        }
        
        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }
    }
}
