using System;
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
        private BatchOneWaySynchronize _batchOneWaySynchronize;
        private Mock<ISyncObjectRepository> _sourceMock;
        private Mock<IConditonalUpdater> _destinationMock;
        private SyncStateStub _syncState;
        private int _batchSize = 100;

        [SetUp]
        public void SetUp()
        {
            new Mock<ISyncObjectRepository>();
            _sourceMock = new Mock<ISyncObjectRepository>();
            _destinationMock = new Mock<IConditonalUpdater>();
            _syncState = new SyncStateStub();
            _batchOneWaySynchronize = new BatchOneWaySynchronize(_sourceMock.Object, _destinationMock.Object, _syncState, _batchSize);
        }

        [Test]
        public void ShouldSynchronize()
        {
            DateTime lastPullDateTime = new DateTime(2012,5,20);
            int lastPulledKey = 4560;

            _syncState.MinimumModificationDate = lastPullDateTime;
            _syncState.CurrentRecord = lastPulledKey;

            var supporter1 = new Supporter{ExternalKey = 4561};
            var supporter2 = new Supporter{ExternalKey = 4562};
            var supporter3 = new Supporter{ExternalKey = 4563};
            IEnumerable<Supporter> pulledObjects1 = new List<Supporter> { supporter1, supporter2 };
            IEnumerable<Supporter> pulledObjects2 = new List<Supporter> { supporter3 };

            _sourceMock.Setup(source => source.GetBatchOfObjects<Supporter>(_batchSize, lastPulledKey, lastPullDateTime)).Returns(pulledObjects1);
            _sourceMock.Setup(source => source.GetBatchOfObjects<Supporter>(_batchSize, supporter2.ExternalKey.Value, lastPullDateTime)).Returns(pulledObjects2);

            _batchOneWaySynchronize.Synchronize<Supporter>();

            _destinationMock.Verify(conditionalUpdater => conditionalUpdater.MaybeUpdate<Supporter>(supporter1));
            _destinationMock.Verify(conditionalUpdater => conditionalUpdater.MaybeUpdate<Supporter>(supporter2));
            _destinationMock.Verify(conditionalUpdater => conditionalUpdater.MaybeUpdate<Supporter>(supporter3));

            Assert.IsTrue(_syncState.PullingCompletedCalled);
        }
    }
}
