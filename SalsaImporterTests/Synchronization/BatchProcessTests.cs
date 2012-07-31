using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class BatchProcessTests
    {
       private BatchProcess _batchProcess;
        private Mock<ISyncObjectRepository> _localRepositoryMock;
        private Mock<ISyncObjectRepository> _externalRepositoryMock;
        private Mock<ISyncLog> _syncLogMock;
        private Mock<IObjectProcess> _objectProcessMock;
        [SetUp]
        public void SetUp()
        {
            _localRepositoryMock = new Mock<ISyncObjectRepository>();
            _externalRepositoryMock = new Mock<ISyncObjectRepository>();
            _syncLogMock = new Mock<ISyncLog>();
            _objectProcessMock = new Mock<IObjectProcess>();
            _batchProcess = new BatchProcess(_localRepositoryMock.Object,
                _externalRepositoryMock.Object, 
                _syncLogMock.Object,
                _objectProcessMock.Object);
        }
        
        [Test]
        public void ShouldPullFromExternal()
        {
            int batchSize = 100;

            DateTime lastPullDateTime = new DateTime(2012,5,20);
            DateTime begin = DateTime.Now;
            DateTime currentDateTime = DateTime.Now;
            _syncLogMock.Setup(log => log.LastPullDateTime).Returns(lastPullDateTime);
            int lastPulledKey = 4560;
            _syncLogMock.Setup(log => log.LastPulledKey).Returns(lastPulledKey);
            _syncLogMock.Setup(log => log.CurrentDateTime).Returns(currentDateTime);
        

            var supporter1 = new Supporter{ExternalKey = 4561};
            var supporter2 = new Supporter{ExternalKey = 4562};
            var supporter3 = new Supporter{ExternalKey = 4563};
            IEnumerable<ISyncObject> pulledObjects1 = new List<Supporter>{supporter1, supporter2};
            IEnumerable<ISyncObject> pulledObjects2 = new List<Supporter>{supporter3};

            _externalRepositoryMock.Setup(external => external.GetBatchOfObjects<Supporter>(batchSize, lastPulledKey, lastPullDateTime)).Returns(pulledObjects1);
            _externalRepositoryMock.Setup(external => external.GetBatchOfObjects<Supporter>(batchSize, supporter2.ExternalKey.Value, currentDateTime)).Returns(pulledObjects2);

            _objectProcessMock.Setup(objectProcess => objectProcess.ProcessPulledObjects(pulledObjects1));
            _objectProcessMock.Setup(objectProcess => objectProcess.ProcessPulledObjects(pulledObjects2));

            _syncLogMock.SetupSet(log => log.LastPullDateTime = currentDateTime);
            _syncLogMock.SetupSet(log => log.LastPulledKey = supporter3.ExternalKey.Value);

            _batchProcess.PullFromExternal<Supporter>(batchSize);

        }
    }
}
