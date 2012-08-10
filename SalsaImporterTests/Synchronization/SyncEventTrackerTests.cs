using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    [Category("Integration")]
    public class SyncEventTrackerTests
    {
        private SessionContext currentSessionContext;
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            TestUtils.ClearAllSessions();
            SyncSession.ClearCache();
            currentSessionContext = SyncSession.CurrentSession().CurrentContext;
        }

        [Test]
        public void ShouldTrackSyncEvent()
        {
            var start = DateTime.Now;
            var tracker = new SyncEventTracker();
            var supporter = new Supporter {Id = 23, ExternalId = 45, First_Name = "peter", Last_Name = "Foo"};
            ISyncObjectRepository salsaRepository = new SalsaRepository(null, null);
            tracker.TrackEvent(new SyncEventArgs{SyncObject = supporter, Destination = salsaRepository, EventType = SyncEventType.Update}, currentSessionContext);
            var syncEvent = Db(db =>db.SyncEvents.Include("SessionContext").FirstOrDefault());
            Assert.IsNotNull(syncEvent);
            Assert.AreEqual(SyncEventType.Update, syncEvent.EventType);
            Assert.AreEqual(23, syncEvent.ObjectId);
            Assert.AreEqual("Supporter", syncEvent.ObjectType);
            Assert.AreEqual(45, syncEvent.ExternalId);
            Assert.AreEqual(supporter.ToString(), syncEvent.Data);
            Assert.AreEqual(salsaRepository.GetType().Name, syncEvent.Destination);
            Assert.AreEqual(currentSessionContext.Id, syncEvent.SessionContext.Id);
            Assert.IsNull(syncEvent.Error);

        }
        [Test]
        public void ShouldTrackSyncEventWithError()
        {
            var tracker = new SyncEventTracker();
            var supporter = new Supporter { Id = 23, ExternalId = 45, First_Name = "peter", Last_Name = "Foo" };
            var error = new ApplicationException("testing error");
            ISyncObjectRepository salsaRepository = new SalsaRepository(null, null);
            tracker.TrackEvent(new SyncEventArgs { SyncObject = supporter, Destination = salsaRepository, EventType = SyncEventType.Error, Error = error}, currentSessionContext);
            var syncEvent = Db(db => db.SyncEvents.Include("SessionContext").FirstOrDefault());
            Assert.IsNotNull(syncEvent);
            Assert.AreEqual(error.ToString(), syncEvent.Error);

        }

        [Test]
        public void ShouldGetSyncEventsForSession()
        {
            var tracker = new SyncEventTracker();
            var supporter1 = new Supporter { Id = 123, ExternalId = 45, First_Name = "peter", Last_Name = "Foo" };
            var supporter2 = new Supporter { Id = 124, ExternalId = 46, First_Name = "joe", Last_Name = "Foo" };
            ISyncObjectRepository salsaRepository = new SalsaRepository(null, null);
            tracker.TrackEvent(new SyncEventArgs { SyncObject = supporter1, Destination = salsaRepository, EventType = SyncEventType.Add}, currentSessionContext);
            tracker.TrackEvent(new SyncEventArgs { SyncObject = supporter2, Destination = salsaRepository, EventType = SyncEventType.Update}, currentSessionContext);

            List<SyncEvent> allEvents = null;
            tracker.SyncEventsForSession(currentSessionContext, events => allEvents = events.ToList());

            Assert.AreEqual(2, allEvents.Count());

            Assert.IsTrue(allEvents.Any(e => e.ObjectId == 123));
            Assert.IsTrue(allEvents.Any(e => e.ObjectId == 124));
        }


        private T Db<T>(Func<AftDbContext, T> func)
        {
            using (var db = new AftDbContext())
            {
                return func(db);
            }
        }
    }
}
