﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class BatchProcessTests
    {
       private BatchProcess _batchProcess;
        private Mock<ISyncObjectRepository> _localRepositoryMock;
        private Mock<ISyncObjectRepository> _externalRepositoryMock;
        private SyncLogStub _syncLog;
        private Mock<IObjectProcess> _objectProcessMock;
        [SetUp]
        public void SetUp()
        {
            _localRepositoryMock = new Mock<ISyncObjectRepository>();
            _externalRepositoryMock = new Mock<ISyncObjectRepository>();
            _syncLog = new SyncLogStub();
            _objectProcessMock = new Mock<IObjectProcess>();
            _batchProcess = new BatchProcess(_localRepositoryMock.Object,
                _externalRepositoryMock.Object, 
                _syncLog,
                _objectProcessMock.Object);
        }

       
        [Test]
        public void ShouldPullFromExternal()
        {
            int batchSize = 100;

            DateTime lastPullDateTime = new DateTime(2012,5,20);
            int lastPulledKey = 4560;

            DateTime begin = DateTime.Now;
            _syncLog.LastPullDateTime = lastPullDateTime;
            _syncLog.LastPulledKey = lastPulledKey;

            var supporter1 = new Supporter{ExternalKey = 4561};
            var supporter2 = new Supporter{ExternalKey = 4562};
            var supporter3 = new Supporter{ExternalKey = 4563};
            IEnumerable<Supporter> pulledObjects1 = new List<Supporter> { supporter1, supporter2 };
            IEnumerable<Supporter> pulledObjects2 = new List<Supporter> { supporter3 };

            _externalRepositoryMock.Setup(external => external.GetBatchOfObjects<Supporter>(batchSize, lastPulledKey, lastPullDateTime)).Returns(pulledObjects1);
            _externalRepositoryMock.Setup(external => external.GetBatchOfObjects<Supporter>(batchSize, supporter2.ExternalKey.Value, lastPullDateTime)).Returns(pulledObjects2);

            _batchProcess.PullFromExternal<Supporter>(batchSize);

            _objectProcessMock.Verify(objectProcess => objectProcess.ProcessPulledObject<Supporter>(supporter1));
            _objectProcessMock.Verify(objectProcess => objectProcess.ProcessPulledObject<Supporter>(supporter2));
            _objectProcessMock.Verify(objectProcess => objectProcess.ProcessPulledObject<Supporter>(supporter3));

            Assert.IsTrue(_syncLog.PullingCompletedCalled);
        }

        [Test]
        public void ShouldPushToExternal()
        {
            int batchSize = 100;

            DateTime lastPushDatetime = new DateTime(2012, 5, 20);
            int lastPushKey = 1230;

            _syncLog.LastPushDateTime = lastPushDatetime;
            _syncLog.LastPushedKey = lastPushKey;

            var supporter1 = new Supporter { ExternalKey = 4561, LocalKey = 1234};
            var supporter2 = new Supporter { ExternalKey = 4562, LocalKey = 1235};
            var supporter3 = new Supporter { ExternalKey = 4563 , LocalKey = 1236};
            IEnumerable<Supporter> batch1 = new List<Supporter> { supporter1, supporter2 };
            IEnumerable<Supporter> batch2 = new List<Supporter> { supporter3 };

            _localRepositoryMock.Setup(local => local.GetBatchOfObjects<Supporter>(batchSize, lastPushKey, lastPushDatetime)).Returns(batch1);
            _localRepositoryMock.Setup(local => local.GetBatchOfObjects<Supporter>(batchSize, supporter2.LocalKey, lastPushDatetime)).Returns(batch2);

            _batchProcess.PushToExternal<Supporter>(batchSize);

            _objectProcessMock.Verify(objectProcess => objectProcess.ProcessPushingObject<Supporter>(supporter1));
            _objectProcessMock.Verify(objectProcess => objectProcess.ProcessPushingObject<Supporter>(supporter2));
            _objectProcessMock.Verify(objectProcess => objectProcess.ProcessPushingObject<Supporter>(supporter3));

            Assert.IsTrue(_syncLog.PushingCompletedCalled);
        }
    }
}
