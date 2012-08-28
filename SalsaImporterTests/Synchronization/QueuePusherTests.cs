using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class QueuePusherTests
    {
        private const int BatchSize = 100;
        private const string ObjectType = "TheObjectType";
        private const string QueueName = "QueueName";

        private QueuePusher _pusher;
        private Mock<ISalsaRepository> _sourceMock;
        private Mock<IQueueRepository> _destinationMock;
        private JobContextStub _jobContext;


        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            _sourceMock = new Mock<ISalsaRepository>();
            _destinationMock = new Mock<IQueueRepository>();
            _jobContext = new JobContextStub();
            _pusher = new QueuePusher(_sourceMock.Object, _destinationMock.Object, BatchSize, null, ObjectType, QueueName);
        }

        [Test]
        public void ShouldSynchronize()
        {
            var minimumModificationDate = new DateTime(2012, 5, 20);
            const int currentRecord = 4560;
         
            _jobContext.MinimumModificationDate = minimumModificationDate;
            _jobContext.SetCurrentRecord(currentRecord);

            var syncObject1 = new SyncObject(ObjectType) { Id = 4561 };
            var syncObject2 = new SyncObject(ObjectType) { Id = 4562 };
            var syncObject3 = new SyncObject(ObjectType) { Id = 4563 };
            IEnumerable<SyncObject> pulledObjects1 = new List<SyncObject> { syncObject1, syncObject2 };
            IEnumerable<SyncObject> pulledObjects2 = new List<SyncObject> { syncObject3 };

            _sourceMock.Setup(source => source.GetBatchOfObjects(ObjectType, BatchSize, currentRecord, minimumModificationDate)).Returns(pulledObjects1);
            _sourceMock.Setup(source => source.GetBatchOfObjects(ObjectType, BatchSize, syncObject2.Id, minimumModificationDate)).Returns(pulledObjects2);

            _pusher.Start(_jobContext);

            _destinationMock.Verify(queueRepository => queueRepository.Push(syncObject1, QueueName));
            _destinationMock.Verify(queueRepository => queueRepository.Push(syncObject2, QueueName));
            _destinationMock.Verify(queueRepository => queueRepository.Push(syncObject3, QueueName));
        }
    }
}
