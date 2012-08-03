﻿using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class BatchOneWaySynchronizeTests
    {
        private BatchOneWaySynchronizer _batchOneWaySynchronizer;
        private Mock<ISyncObjectRepository> _sourceMock;
        private Mock<IConditonalUpdater> _destinationMock;
        private JobContextStub _jobContext;
        private const int BatchSize = 100;

        [SetUp]
        public void SetUp()
        {
            _sourceMock = new Mock<ISyncObjectRepository>();
            _destinationMock = new Mock<IConditonalUpdater>();
            _jobContext = new JobContextStub();
            _batchOneWaySynchronizer = new BatchOneWaySynchronizer(_sourceMock.Object, _destinationMock.Object, _jobContext, BatchSize);
        }

        [Test]
        public void ShouldSynchronize()
        {
            var minimumModificationDate = new DateTime(2012,5,20);
            const int currentRecord = 4560;

            _jobContext.MinimumModificationDate = minimumModificationDate;
            _jobContext.CurrentRecord = currentRecord;

            var supporter1 = new Supporter{Id = 4561};
            var supporter2 = new Supporter{Id = 4562};
            var supporter3 = new Supporter{Id = 4563};
            IEnumerable<Supporter> pulledObjects1 = new List<Supporter> { supporter1, supporter2 };
            IEnumerable<Supporter> pulledObjects2 = new List<Supporter> { supporter3 };

            _sourceMock.Setup(source => source.GetBatchOfObjects<Supporter>(BatchSize, currentRecord, minimumModificationDate)).Returns(pulledObjects1);
            _sourceMock.Setup(source => source.GetBatchOfObjects<Supporter>(BatchSize, supporter2.Id, minimumModificationDate)).Returns(pulledObjects2);

            _batchOneWaySynchronizer.Synchronize<Supporter>();

            _destinationMock.Verify(conditionalUpdater => conditionalUpdater.MaybeUpdate<Supporter>(supporter1));
            _destinationMock.Verify(conditionalUpdater => conditionalUpdater.MaybeUpdate<Supporter>(supporter2));
            _destinationMock.Verify(conditionalUpdater => conditionalUpdater.MaybeUpdate<Supporter>(supporter3));

            Assert.IsTrue(_jobContext.PullingCompletedCalled);
        }
    }
}
