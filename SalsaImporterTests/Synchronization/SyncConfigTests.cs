using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncConfigTests
    {
        [Test]
        public void ShouldGetQueueName()
        {
            var syncJob = new SyncConfig {SyncDirection = SyncDirection.Export, ObjectType = "testObjectType"};
            Assert.AreEqual("AftToSalsaQueue_testObjectType", syncJob.QueueName);
        }

        [Test]
        public void ShouldGetQueueHistoryName()
        {
            var syncJob = new SyncConfig { SyncDirection = SyncDirection.Export, ObjectType = "testObjectType" };
            Assert.AreEqual("AftToSalsaQueue_testObjectType_History", syncJob.QueueHistoryName);
        }
    }
}
