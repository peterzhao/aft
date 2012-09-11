using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class QueueRepositoryTests
    {
        private const string TableName = "SalsaToAftQueue_Supporter";
        private const string ObjectType = "supporter";
        private QueueRepository _repository;
        private SyncObject _syncObject;
        private Mock<IMapperFactory> _mapperFactoryMock;
        private IMapper _mapper;
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            _mapperFactoryMock = new Mock<IMapperFactory>();
            var fieldMapping1 = new FieldMapping {AftField = "First_Name", SalsaField = "First_Name", DataType = "string"};
            var fieldMapping2 = new FieldMapping {AftField = "Last_Name", SalsaField = "Last_Name", DataType = "string"};
            _mapper = new Mapper(ObjectType, new List<FieldMapping>{fieldMapping1, fieldMapping2});
            _repository = new QueueRepository(_mapperFactoryMock.Object);
            _syncObject = new SyncObject(ObjectType);

            TestUtils.ClearAllQueues();
        }

      

        [Test]
        public void ShouldInsertObejctIntoQueue()
        {
            var startDate = DateTime.Now;
            const string expectedFirstName = "firstname";
            const string expectedEmail = "foo@abc.com";

            _syncObject["First_Name"] = expectedFirstName;
            _syncObject["Email"] = expectedEmail;
            _syncObject.SalsaKey = 1234;
            _repository.Enqueue(TableName, _syncObject);

            List<Dictionary<string, object>> queueData = TestUtils.ReadAllFromTable(TableName);

            Assert.AreEqual(1, queueData.Count);
            Dictionary<string, object> firstQueueRecord = queueData.First();

            Assert.AreEqual(expectedFirstName, firstQueueRecord["First_Name"]);
            Assert.AreEqual(expectedEmail, firstQueueRecord["Email"]);
            Assert.AreEqual(1234, firstQueueRecord["SalsaKey"]);

        }

        [Test]
        public void ShouldRaiseEventWhenPushed()
        {
            SyncEventArgs eventArgs = null;
            _repository.NotifySyncEvent += (sender, args) => eventArgs = args;
            _repository.Enqueue(TableName, _syncObject);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(_syncObject.ObjectType, eventArgs.ObjectType);
            Assert.AreEqual(SyncEventType.Import, eventArgs.EventType);
            Assert.AreEqual(_repository, eventArgs.Destination);
        }

        [Test]
        public void ShouldGetBatchOfObjects()
        {
            _mapperFactoryMock.Setup(factory => factory.GetMapper(ObjectType)).Returns(_mapper);
            Enqueue("foo@abc.com", "peter", "zhao");
            Enqueue("foo2@abc.com", "peter2", "zhao2");
            Enqueue("foo3@abc.com", "peter3", "zhao3");

            var batch1 = _repository.GetBatchOfObjects(ObjectType, TableName, 2, 0);
            Assert.AreEqual(2, batch1.Count);
            Assert.AreEqual("peter", batch1.First()["First_Name"]);
            Assert.AreEqual("zhao", batch1.First()["Last_Name"]);

            Assert.AreEqual("peter2", batch1.Last()["First_Name"]);
            Assert.AreEqual("zhao2", batch1.Last()["Last_Name"]);

            var batch2 = _repository.GetBatchOfObjects(ObjectType, TableName, 2, batch1.Last().QueueId);
            Assert.AreEqual(1, batch2.Count);
            Assert.AreEqual("peter3", batch2.First()["First_Name"]);
            Assert.AreEqual("zhao3", batch2.First()["Last_Name"]);


           var batch3 = _repository.GetBatchOfObjects(ObjectType, TableName, 2, batch2.Last().QueueId); 
            Assert.AreEqual(0, batch3.Count);
        }

        [Test]
        public void ShouldNotReadNullFieldsIntoSyncObject()
        {
            _mapperFactoryMock.Setup(factory => factory.GetMapper(ObjectType)).Returns(_mapper);
            Enqueue("foo@abc.com", "peter");

            var batch1 = _repository.GetBatchOfObjects(ObjectType, TableName, 1, 0);
            Assert.AreEqual(1, batch1.Count);
            Assert.AreEqual("peter", batch1.First()["First_Name"]);
            Assert.IsFalse(batch1.First().FieldNames.Contains("Last_Name"));
        }

        [Test]
        public void ShouldUpdateStatusAndProcessedDate()
        {
            Enqueue("foo1@abc.com", "peter");
            Enqueue("foo2@abc.com", "jim");
            var rows = TestUtils.ReadAllFromTable(TableName);
            var processedDate = DateTime.Now;
            _repository.UpdateStatus(TableName, int.Parse(rows.First()["Id"].ToString()), "imported", processedDate);
            var newRows = TestUtils.ReadAllFromTable(TableName);
            Assert.AreEqual("imported", newRows.First()["Status"]);
            Assert.AreEqual(processedDate.ToString(), newRows.First()["ProcessedDate"].ToString());
        }

        [Test]
        public void ShouldUpdateStatusOnly()
        {
            Enqueue("foo1@abc.com", "peter");
            Enqueue("foo2@abc.com", "jim");
            var rows = TestUtils.ReadAllFromTable(TableName);
            _repository.UpdateStatus(TableName, int.Parse(rows.First()["Id"].ToString()), "imported");
            var newRows = TestUtils.ReadAllFromTable(TableName);
            Assert.AreEqual("imported", newRows.First()["Status"]);
            Assert.AreEqual(DBNull.Value, newRows.First()["ProcessedDate"]);
        }

        [Test]
        public void ShouldDequeueSyncObjectAndMoveToHistoryTable()
        {
            Enqueue("foo1@abc.com", "peter");
            Enqueue("foo2@abc.com", "jim");
            var rows = TestUtils.ReadAllFromTable(TableName);
            _repository.Dequeue(TableName,int.Parse(rows.First()["Id"].ToString()));

            var historyRows = TestUtils.ReadAllFromTable(TableName + "_History");

            Assert.AreEqual(1, TestUtils.ReadAllFromTable(TableName).Count);
            Assert.AreEqual(1, TestUtils.ReadAllFromTable(TableName+"_History").Count);
            Assert.AreEqual(rows.First()["Email"], historyRows.First()["Email"]);
            Assert.AreEqual(rows.First()["First_Name"], historyRows.First()["First_Name"]);
        }

        private void Enqueue(string email, string firstName, string lastName)
        {
            TestUtils.ExecuteSql(string.Format("insert into {0} ([First_Name],[Last_Name],[Email], SalsaKey) VALUES('{1}','{2}','{3}', 0)", 
                TableName, firstName, lastName, email));
        }

        private void Enqueue(string email, string firstName)
        {
            TestUtils.ExecuteSql(string.Format("insert into {0} ([First_Name],[Email], SalsaKey) VALUES('{1}','{2}', 0)",
                TableName, firstName, email));
        }

    }
}
