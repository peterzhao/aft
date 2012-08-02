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
        private ObjectProcess _objectProcess;

        [SetUp]
        public void SetUp()
        {
            _localRepositoryMock = new Mock<ISyncObjectRepository>();
            _externalRepositoryMock = new Mock<ISyncObjectRepository>();
            _errorHandlerMock = new Mock<ISyncErrorHandler>();
            _objectProcess = new ObjectProcess(_localRepositoryMock.Object, _externalRepositoryMock.Object, _errorHandlerMock.Object);
        }
     

        [Test]
        public void ShouldAddToLocalIfExternalCannotMatchToLocal()
        {
            var externalObj = new Supporter {ExternalKey = 1234};
            int newlocalId = 3456;
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(()=> null);
            _localRepositoryMock.Setup(localRepository => localRepository.Add(externalObj)).Returns(newlocalId);

            _objectProcess.ProcessPulledObject<Supporter>(externalObj);
        }

        [Test]
        public void ShouldDoNothingIfExternalIsSameAsLocal()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, LocalModifiedDate = new DateTime(2012, 7, 21)};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            _objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _externalRepositoryMock.Verify(externalRepository => externalRepository.Update(externalObj, localObj), Times.Never());
            _localRepositoryMock.Verify(localRepository => localRepository.Add(externalObj), Times.Never());
        }

        [Test]
        public void ShouldUpdateLocalObjectIfExternalIsNewerThanLocal()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, 
                Phone = "4161234567", ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678,
                Phone = "4161234568", LocalModifiedDate = new DateTime(2012, 6, 23)};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            _objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _localRepositoryMock.Verify(localRepository => localRepository.Update(externalObj, localObj));
        }


        [Test]
        public void ShouldHandleErrorWhenProcessPulledObject()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678 };
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value))
                .Throws<Exception>();

            _objectProcess.ProcessPulledObject<Supporter>(externalObj);

            _errorHandlerMock.Verify(errorHandler => errorHandler.HandlePullObjectFailure(externalObj, It.IsAny<Exception>()));
        }


        [Test]
        public void ShouldAddToExternalIfLocalObjectHasNoExternalKey()
        {
            var localObj = new Supporter { LocalKey = 1234 };
            _externalRepositoryMock.Setup(externalRepository => externalRepository.Add(localObj)).Returns(3456);
        
            _objectProcess.ProcessPushingObject<Supporter>(localObj);
            _localRepositoryMock.Verify(localRepositoy => localRepositoy.Update(It.IsAny<ISyncObject>(), localObj));
        }

        [Test]
        public void ShouldAddToExternalIfThereIsNoMatchedExternalObject() //Todo: actual it should handle delete logic here. but we don't know this moment.
        {
            var localObj = new Supporter { LocalKey = 1234, ExternalKey = 7856};
            int newExternalId = 3456;
            _externalRepositoryMock.Setup(externalRepository => externalRepository.Get<Supporter>(localObj.ExternalKey.Value)).Returns(() => null);
            _externalRepositoryMock.Setup(externalRepository => externalRepository.Add(localObj)).Returns(newExternalId);

            _objectProcess.ProcessPushingObject<Supporter>(localObj);
            _localRepositoryMock.Verify(localRepositoy => localRepositoy.Update(It.IsAny<ISyncObject>(), localObj));
        }
        
        [Test]
        public void ShouldDoNothingIfLacalIsSameAsExternal()
        {
            var localObj = new Supporter { LocalKey = 1234, ExternalKey = 7856 };
            var externalObj = new Supporter { LocalKey = 1234, ExternalKey = 7856 };
            _externalRepositoryMock.Setup(externalRepository => externalRepository.Get<Supporter>(localObj.ExternalKey.Value)).Returns(() => externalObj);

            _objectProcess.ProcessPushingObject<Supporter>(localObj);
            _localRepositoryMock.Verify(localRepositoy => localRepositoy.Update(It.IsAny<ISyncObject>(), localObj), Times.Never());
            _externalRepositoryMock.Verify(externalRepository => externalRepository.Add(localObj), Times.Never());
        }
        
        [Test]
        public void ShouldUpdateExternalObjectIfLocalIsNewerThanExternal()
        {
         var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, 
                Phone = "4161234567", ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678,
                Phone = "4161234568", LocalModifiedDate = new DateTime(2012, 8, 23)};
            _externalRepositoryMock.Setup(externalRepository => externalRepository.Get<Supporter>(localObj.ExternalKey.Value)).Returns(externalObj);

            _objectProcess.ProcessPushingObject<Supporter>(localObj);

            _externalRepositoryMock.Verify(externalRepository => externalRepository.Update<Supporter>(localObj, externalObj));
        }
        
        
        [Test]
        public void ShouldHandleErrorWhenProcessPushingObject()
        {
            var localObj = new Supporter { LocalKey = 1234, ExternalKey = 7856, LocalModifiedDate = new DateTime(2012, 7, 23) };
            _externalRepositoryMock.Setup(externalRepository => externalRepository.Get<Supporter>(localObj.ExternalKey.Value)).Throws<Exception>();

            _objectProcess.ProcessPushingObject<Supporter>(localObj);
            _errorHandlerMock.Verify(errorHandler => errorHandler.HandlePullObjectFailure(localObj, It.IsAny<Exception>()));
        }
      
    }
}
