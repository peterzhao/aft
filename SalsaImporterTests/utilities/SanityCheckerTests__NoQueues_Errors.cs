using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SanityCheckerTests__NoQueues_Errors
    {
        private List<string> _result;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            TestUtils.RemoveAllSyncConfigs();
            TestUtils.CreateSyncConfig("donation", SyncDirection.Export, 1);
            TestUtils.CreateSyncConfig("donation", SyncDirection.Import, 2);

            _result = new SanityChecker().Verify();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.CreateDefaultSyncConfigs();
        }

        [Test]
        public void ShouldCheckQueueExisting()
        {
            Assert.IsTrue(_result.Any(m => m.Equals("Could not find the table for the queue AftToSalsaQueue_donation")));
            Assert.IsTrue(_result.Any(m => m.Equals("Could not find the table for the queue SalsaToAftQueue_donation")));
        }
        [Test]
        public void ShouldCheckHistoryTableExisting()
        {
            Assert.IsTrue(_result.Any(m=> m.Equals("Could not find the history table SalsaToAftQueue_donation_History")));
            Assert.IsTrue(_result.Any(m=> m.Equals("Could not find the history table AftToSalsaQueue_donation_History")));
        }
    }
}