
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class QueueRepositoryTests
    {
        private const string TableName = "theQueue";
        private Mock<IAftDbContext> _dbMock;
        private QueueRepository _repository;
        private SyncObject _syncObject;

        [SetUp]
        public void SetUp()
        {

            _dbMock = new Mock<IAftDbContext>();
            _repository = new QueueRepository(_dbMock.Object);
            _syncObject = new SyncObject("supporter");
        }

        [Test]
        public void ShouldInsertDataIntoDb()
        {
            _syncObject["Address"] = "100 main st";
            _syncObject["Email"] = "foo@abc.com";

            _repository.Push(_syncObject, TableName);

            _dbMock.Verify(db => db.InsertToQueue(_syncObject, 
                TableName,
                It.Is<List<string>>(l => l.Contains("Email") && l.Contains("Address"))));

        }

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
