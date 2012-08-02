using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;
using SalsaImporterTests.utilities;

namespace SalsaImporterTests.Synchronization
{
    class PullSalsaToLocalTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.UnitTest;

            TestUtils.RemoveAll<Supporter>();
            TestUtils.RemoveAll<SyncRun>();
        }

        [Test]
        public void ShouldPullNewSubscribersToLocalDb()
        {
            AssertLocalSubscriberCount(0);

            var salsaClient = new SalsaClient(new SyncErrorHandler(10,10));
            var salsaSupporterCount = salsaClient.SupporterCount();

            var pullSalsaToLocal = new PullSalsaToLocal();
            pullSalsaToLocal.run();

            AssertLocalSubscriberCount(salsaSupporterCount);
        }

        private static void AssertLocalSubscriberCount(int supporterCount)
        {
            using (var db = new AftDbContext())
            {
                Assert.AreEqual(supporterCount, db.Supporters.Count());
            }
        }
    }
}
