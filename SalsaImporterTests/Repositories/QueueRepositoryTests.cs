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
        private const string ObjectType = "supporter";
        private QueueRepository _repository;
        private SyncObject _syncObject;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;

            _repository = new QueueRepository();
            _syncObject = new SyncObject(ObjectType);

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
            Assert.AreEqual(SyncEventType.Import, eventArgs.EventType);
            Assert.AreEqual(_repository, eventArgs.Destination);
        }

        [Test]
        public void ShouldDequeueBatchOfObjects()
        {
            Enqueue("foo@abc.com", "peter", "zhao");
            Enqueue("foo2@abc.com", "peter2", "zhao2");
            Enqueue("foo3@abc.com", "peter3", "zhao3");
            var batch1 = _repository.DequequBatchOfObjects(ObjectType, TableName, 2, 0);
            Assert.AreEqual(1, TestUtils.ReadAllFromQueue(TableName).Count);
            Assert.AreEqual(2, batch1.Count);
            Assert.AreEqual("foo@abc.com", batch1.First()["Email"]);
            Assert.AreEqual("peter", batch1.First()["First_Name"]);
            Assert.AreEqual("zhao", batch1.First()["Last_Name"]);

            Assert.AreEqual("foo2@abc.com", batch1.Last()["Email"]);
            Assert.AreEqual("peter2", batch1.Last()["First_Name"]);
            Assert.AreEqual("zhao2", batch1.Last()["Last_Name"]);

            var batch2 = _repository.DequequBatchOfObjects(ObjectType, TableName, 2, batch1.Last().Id);
            Assert.AreEqual(1, batch2.Count);
            Assert.AreEqual("foo3@abc.com", batch2.First()["Email"]);
            Assert.AreEqual("peter3", batch2.First()["First_Name"]);
            Assert.AreEqual("zhao3", batch2.First()["Last_Name"]);

            Assert.AreEqual(0, TestUtils.ReadAllFromQueue(TableName).Count);
        }

        private void Enqueue(string email, string firstName, string lastName)
        {
            TestUtils.ExecuteSql(string.Format("insert into {0} ([First_Name],[Last_Name],[Email], SalsaKey) VALUES('{1}','{2}','{3}', 0)", 
                TableName, firstName, lastName, email));
        }
    }
}
