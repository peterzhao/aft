using System;
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
    public class ObjectProcessTests
    {
        private Mock<ISyncObjectRepository> _localRepositoryMock;
        private Mock<ISyncObjectRepository> _externalRepositoryMock;
        private Mock<ISyncErrorHandler> _errorHandlerMock;
        private ObjectProcess objectProcess;

        [SetUp]
        public void SetUp()
        {
            _localRepositoryMock = new Mock<ISyncObjectRepository>();
            _externalRepositoryMock = new Mock<ISyncObjectRepository>();
            _errorHandlerMock = new Mock<ISyncErrorHandler>();
            objectProcess = new ObjectProcess(_localRepositoryMock.Object, _externalRepositoryMock.Object, _errorHandlerMock.Object);
        }
     

        [Test]
        public void ShouldAddToLocalIfExternalCannotMatchToLocal()
        {
            var externalObj = new Supporter {ExternalKey = 1234};
            int newlocalId = 3456;
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(()=> null);
            _localRepositoryMock.Setup(localRepository => localRepository.Add(externalObj)).Returns(newlocalId);

            objectProcess.ProcessPulledObject<Supporter>(externalObj);
            _externalRepositoryMock.Verify(externalRepository => externalRepository.Update(It.IsAny<ISyncObject>(), externalObj));
        }

        [Test]
        public void ShouldDoNothingIfExternalIsSameAsLocal()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, localModifiedDate = new DateTime(2012, 7, 21)};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _externalRepositoryMock.Verify(externalRepository => externalRepository.Update(externalObj, localObj), Times.Never());
            _localRepositoryMock.Verify(localRepository => localRepository.Add(externalObj), Times.Never());
        }

        [Test]
        public void ShouldUpdateLocalObjectIfExternalIsNewerThanLocal()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, 
                Phone = "4161234567", ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678,
                Phone = "4161234568", localModifiedDate = new DateTime(2012, 6, 23)};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _localRepositoryMock.Verify(localRepository => localRepository.Update(externalObj, localObj));
        }

        [Test]
        public void ShouldUpdateExternalObjectIfExternalIsOlderThanLocal()
        {
            var externalObj = new Supporter
            {
                ExternalKey = 1234,
                Email = "jj@abc.com",
                LocalKey = 5678,
                Phone = "4161234567",
                ExternalModifiedDate = new DateTime(2012, 3, 20)
            };
            var localObj = new Supporter
            {
                ExternalKey = 1234,
                Email = "jj@abc.com",
                LocalKey = 5678,
                Phone = "4161234568",
                localModifiedDate = new DateTime(2012, 6, 23)
            };
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _externalRepositoryMock.Verify(externalRepository => externalRepository.Update(localObj, externalObj));

        }

        [Test]
        public void ShouldHandleError()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678 };
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value))
                .Throws<Exception>();

            objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _errorHandlerMock.Verify(errorHandler => errorHandler.HandlePullObjectFailure(externalObj, It.IsAny<Exception>()));
        }
      
    }
}
