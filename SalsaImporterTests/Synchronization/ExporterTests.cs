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
    public class ExporterTests
    {
        private const int BatchSize = 100;
        private const string ObjectType = "TheObjectType";
        private const string QueueName = "QueueName";

        private Exporter _exporter;
        private Mock<ISalsaRepository> _destination;
        private Mock<IQueueRepository> _source;
        private JobContextStub _jobContext;


        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            _destination = new Mock<ISalsaRepository>();
            _source = new Mock<IQueueRepository>();
            _jobContext = new JobContextStub();
            _exporter = new Exporter(_source.Object, _destination.Object, BatchSize, "testJob", ObjectType, QueueName);
        }

        [Test]
        public void ShouldSynchronize()
        {
            const int currentRecord = 4560;

            _jobContext.SetCurrentRecord(currentRecord);

            var syncObject1 = new SyncObject(ObjectType) { Id = 4561 };
            var syncObject2 = new SyncObject(ObjectType) { Id = 4562 };
            var syncObject3 = new SyncObject(ObjectType) { Id = 4563 };
            var pulledObjects1 = new List<SyncObject> { syncObject1, syncObject2 };
            var pulledObjects2 = new List<SyncObject> { syncObject3 };

            _source.Setup(source => source.DequequBatchOfObjects(ObjectType, QueueName,BatchSize, currentRecord)).Returns(pulledObjects1);
            _source.Setup(source => source.DequequBatchOfObjects(ObjectType, QueueName, BatchSize, syncObject2.Id)).Returns(pulledObjects2);
            _source.Setup(source => source.DequequBatchOfObjects(ObjectType, QueueName, BatchSize, syncObject3.Id)).Returns(new List<SyncObject>());

            _exporter.Start(_jobContext);

            _destination.Verify(destination => destination.Save(syncObject1));
            _destination.Verify(destination => destination.Save(syncObject2));
            _destination.Verify(destination => destination.Save(syncObject3));
        }
    }
}
