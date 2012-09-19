using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class LogTrimmerTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }
        [Test]
        public void ShouldRemoveOlderImporterLogs()
        {
            var oneDaysAgo = DateTime.Now.AddDays(-1).ToString();
            var twoDaysAgo = DateTime.Now.AddDays(-2).ToString();
            TestUtils.ExecuteSql("delete from importerLogs");
            TestUtils.ExecuteSql(string.Format("insert into importerLogs (time_stamp, level, message, threadid) values('{0}','debug','test1', 1)", oneDaysAgo));
            TestUtils.ExecuteSql(string.Format("insert into importerLogs (time_stamp, level, message, threadid) values('{0}','debug','test2', 1)", twoDaysAgo));
            new LogTrimmer().TrimImporterLogsOlderThan(1);

            var rows = TestUtils.ReadAllFromTable("importerLogs");
            Assert.IsTrue(rows.Any(r => r["message"].Equals("test1")));
            Assert.IsFalse(rows.Any(r => r["message"].Equals("test2")));
        }

        [Test]
        public void ShouldRemoveOlderSyncEventsExludingErrors()
        {
            var oneDaysAgo = DateTime.Now.AddDays(-1);
            var session1 = new SessionContext {State = "finished", StartTime = oneDaysAgo, MinimumModifiedDate = oneDaysAgo};
            var session2 = new SessionContext {State = "started", StartTime = DateTime.Now, MinimumModifiedDate = oneDaysAgo};

            using(var db = new AftDbContext())
            {
                db.SessionContexts.Add(session1);
                db.SaveChanges();
                db.SessionContexts.Add(session2);
                db.SaveChanges();
            }
            TestUtils.ExecuteSql("delete from syncEvents");
            TestUtils.ExecuteSql(string.Format("insert into syncEvents (eventType, data, salsaKey, sessionContext_id) values('import','test1',0, {0})", session1.Id));
            TestUtils.ExecuteSql(string.Format("insert into syncEvents (eventType, data, salsaKey, sessionContext_id) values('error','test2',0, {0})", session1.Id));
            TestUtils.ExecuteSql(string.Format("insert into syncEvents (eventType, data, salsaKey, sessionContext_id) values('import','test3',0, {0})", session2.Id));
            new LogTrimmer().TrimOldNonErrorSyncEvents(session2.Id);

            var rows = TestUtils.ReadAllFromTable("syncEvents");
            Assert.IsTrue(rows.Any(r => r["Data"].Equals("test2")));
            Assert.IsTrue(rows.Any(r => r["Data"].Equals("test3")));
            Assert.IsFalse(rows.Any(r => r["Data"].Equals("test1")));
        }
    }
}
