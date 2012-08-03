using System;
using System.Linq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncStateTests
    {
        private AftDbContext db;
        [SetUp]
        public void SetUp()
        {
            using (db = new AftDbContext())
            {
                db.SyncRuns.ToList().ForEach(s => db.SyncRuns.Remove(s));
                db.SaveChanges();
            }
        }
       
        [Test]
        public void ShouldGiveMinimumTimeWhenNoPriorSynchronizations()
        {
            var syncState = new SyncState(DateTime.Now);
            Assert.AreEqual(DateTime.MinValue, syncState.MinimumModificationDate);
        }

        [Test]
        public void ShouldGiveLastStartTimeWhenPriorCompletedSynchronization()
        {
            DateTime firstStartTime = DateTime.Now;
            var firstSyncState = new SyncState(firstStartTime);
            
            firstSyncState.MarkComplete();

            var secondSyncState = new SyncState(DateTime.Now);

            Assert.AreEqual(firstStartTime, secondSyncState.MinimumModificationDate);
        }

        [Test]
        public void ShouldGiveLastKeyZeroAsStartKeyWhenNoPriorSynchronizations()
        {
            var syncState = new SyncState(DateTime.Now);
            Assert.AreEqual(0, syncState.CurrentRecord);
        }

        [Test]
        public void ShouldRetainLastKey()
        {
            var syncState = new SyncState(DateTime.Now);
            syncState.CurrentRecord = 100;
            Assert.AreEqual(100, syncState.CurrentRecord);
        }


        [Test]
        public void ShouldResumeToLastKey()
        {
            var firstSyncState = new SyncState(DateTime.Now);
            firstSyncState.CurrentRecord = 100;
            
            var secondSyncState = new SyncState(DateTime.Now);
            Assert.AreEqual(100, secondSyncState.CurrentRecord);
        }

        [Test]
        public void ShouldStartAtKeyZeroWhenPriorRunComplete()
        {
            var firstSyncState = new SyncState(DateTime.Now);
            firstSyncState.CurrentRecord = 100;
            firstSyncState.MarkComplete();

            var secondSyncState = new SyncState(DateTime.Now);
            Assert.AreEqual(0, secondSyncState.CurrentRecord);
        }
    }
}
