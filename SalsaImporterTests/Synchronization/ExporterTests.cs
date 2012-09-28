using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class ExporterTests
    {
        private const int BatchSize = 100;
        private const string ObjectType = "TheObjectType";
        private const string QueueName = "QueueName";

        private Exporter _exporter;
        private Mock<ISalsaRepository> _destination;
        private Mock<IQueueRepository> _source;
        private Mock<ISyncErrorHandler> _errorHandler;
        private JobContextStub _jobContext;


        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            _destination = new Mock<ISalsaRepository>();
            _source = new Mock<IQueueRepository>();
            _jobContext = new JobContextStub();
            _errorHandler = new Mock<ISyncErrorHandler>();
            _exporter = new Exporter(_source.Object, _destination.Object, _errorHandler.Object, BatchSize, "testJob", ObjectType, QueueName);
        }

        [Test]
        public void ShouldExportObjectsFromQueue()
        {
            const int currentRecord = 4560;

            _jobContext.SetCurrentRecord(currentRecord);

            var syncObject1 = new SyncObject(ObjectType) { QueueId = 4561 };
            var syncObject2 = new SyncObject(ObjectType) { QueueId = 4562 };
            var syncObject3 = new SyncObject(ObjectType) { QueueId = 4563 };
            var pulledObjects1 = new List<SyncObject> { syncObject1, syncObject2 };
            var pulledObjects2 = new List<SyncObject> { syncObject3 };

            _source.Setup(source => source.GetBatchOfObjects(ObjectType, QueueName, BatchSize, currentRecord)).Returns(pulledObjects1);
            _source.Setup(source => source.GetBatchOfObjects(ObjectType, QueueName, BatchSize, syncObject2.QueueId)).Returns(pulledObjects2);
            _source.Setup(source => source.GetBatchOfObjects(ObjectType, QueueName, BatchSize, syncObject3.QueueId)).Returns(new List<SyncObject>());

            var exception = new SaveToSalsaException("test failure");
            _destination.Setup(d => d.Save(syncObject1)).Throws(exception);
            _destination.Setup(d => d.Save(syncObject2)).Returns(true);
            _destination.Setup(d => d.Save(syncObject3)).Returns(false);
            _exporter.Start(_jobContext);

            _errorHandler.Verify(h => h.HandleSyncObjectFailure(syncObject1, _exporter, exception));

            _source.Verify(s => s.UpdateStatus(QueueName, 4561, QueueRepository.QueueStatusError, null));
            _source.Verify(s => s.UpdateStatus(QueueName, 4562, QueueRepository.QueueStatusExported, It.IsAny<DateTime>()));
            _source.Verify(s => s.UpdateStatus(QueueName, 4563, QueueRepository.QueueStatusSkipped, It.IsAny<DateTime>()));
            _source.Verify(s => s.Dequeue(QueueName, 4561), Times.Never());
            _source.Verify(s => s.Dequeue(QueueName, 4562));
            _source.Verify(s => s.Dequeue(QueueName, 4563));

            Assert.AreEqual(1, _jobContext.SuccessCount);
            Assert.AreEqual(1, _jobContext.IdenticalObjectCount);
        }
    }
}
