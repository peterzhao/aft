using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class QueueRepositoryTests
    {
        private const string TableName = "Supporter_SalsaToAftQueue";
        private QueueRepository _repository;
        private SyncObject _syncObject;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;

            _repository = new QueueRepository();
            _syncObject = new SyncObject("supporter");

            TestUtils.ClearAllQueues();
        }

        [Test]
        public void ShouldSetSalsaKey()
        {
            _syncObject.Id = 1234;
            _syncObject["Email"] = "test@example.com";
            
            _repository.Push(_syncObject, TableName);

            List<Dictionary<string, object>> queueData = TestUtils.ReadAllFromQueue(TableName);

            Assert.AreEqual(1, queueData.Count);
            Dictionary<string, object> firstQueueRecord = queueData.First();

            Assert.AreEqual(1234, firstQueueRecord["SalsaKey"]);
        }

        [Test]
        public void ShouldInsertDataIntoDb()
        {
            const string expectedFirstName = "firstname";
            const string expectedEmail = "foo@abc.com";

            _syncObject["First_Name"] = expectedFirstName;
            _syncObject["Email"] = expectedEmail;

            _repository.Push(_syncObject, TableName);

            List<Dictionary<string, object>> queueData = TestUtils.ReadAllFromQueue(TableName);

            Assert.AreEqual(1, queueData.Count);
            Dictionary<string, object> firstQueueRecord = queueData.First();

            Assert.AreEqual(expectedFirstName, firstQueueRecord["First_Name"]);
            Assert.AreEqual(expectedEmail, firstQueueRecord["Email"]);
        }

       

//        [Test]
//        public void ShouldInsertNullSupporter()
//        {
//            var supporter = new SyncObject("supporter");
//            supporter["First_Name"] = null;
//
//            var fields = new List<string>() { "First_Name" };
//
//            QueueRepository.InsertToQueue(_dbContext.Database, supporter, "Supporter_SalsaToAftQueue", fields);
//
//        }
//
//        [Test]
//        public void ShouldInsertEmptySupporter()
//        {
//            var supporter = new SyncObject("supporter");
//            supporter["First_Name"] = "";
//
//            var fields = new List<string>() { "First_Name" };
//
//            QueueRepository.InsertToQueue(_dbContext.Database, supporter, "Supporter_SalsaToAftQueue", fields);
//
//        }

        [Test]
        public void ShouldRaiseEventWhenPushed()
        {
            SyncEventArgs eventArgs = null;
            _repository.NotifySyncEvent += (sender, args) => eventArgs = args;
            _repository.Push(_syncObject, TableName);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(_syncObject.ObjectType, eventArgs.ObjectType);
            Assert.AreEqual(SyncEventType.Add, eventArgs.EventType);
            Assert.AreEqual(_repository, eventArgs.Destination);
        }

    }
}
