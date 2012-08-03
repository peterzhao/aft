using System;
using Moq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class ConditionalUpdaterTests
    {
        private Mock<ISyncObjectRepository> _localRepositoryMock;
        private Mock<ISyncErrorHandler> _errorHandlerMock;
        private ConditionalUpdater _conditionalUpdater;

        [SetUp]
        public void SetUp()
        {
            _localRepositoryMock = new Mock<ISyncObjectRepository>();
            _errorHandlerMock = new Mock<ISyncErrorHandler>();
            _conditionalUpdater = new ConditionalUpdater(_localRepositoryMock.Object, _errorHandlerMock.Object);
        }
     

        [Test]
        public void ShouldAddIfSourceObjectDoesNotExistDestination()
        {
            var sourceObject = new Supporter {ExternalKey = 1234};
            int newlocalId = 3456;
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(sourceObject.ExternalKey.Value)).Returns(()=> null);
            _localRepositoryMock.Setup(localRepository => localRepository.Add(sourceObject)).Returns(newlocalId);

            _conditionalUpdater.MaybeUpdate<Supporter>(sourceObject);

            _localRepositoryMock.Verify(localRepository => localRepository.Add(sourceObject));
        }

        [Test]
        public void ShouldDoNothingIfExternalIsSameAsLocal()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            _conditionalUpdater.MaybeUpdate<Supporter>(externalObj);

            _localRepositoryMock.Verify(localRepository => localRepository.Add(externalObj), Times.Never());
            _localRepositoryMock.Verify(localRepository => localRepository.Update(externalObj, localObj), Times.Never());
        }

        [Test]
        public void ShouldUpdateLocalObjectIfExternalIsNewerThanLocal()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678, 
                Phone = "4161234567", ExternalModifiedDate = new DateTime(2012, 7, 20)};
            var localObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678,
                Phone = "4161234568", LocalModifiedDate = new DateTime(2012, 6, 23)};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value)).Returns(localObj);

            _conditionalUpdater.MaybeUpdate<Supporter>(externalObj);

            _localRepositoryMock.Verify(localRepository => localRepository.Update(externalObj, localObj));
        }

        [Test]
        public void ShouldHandleError()
        {
            var externalObj = new Supporter { ExternalKey = 1234, Email = "jj@abc.com", LocalKey = 5678 };
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.ExternalKey.Value))
                .Throws<Exception>();

            _conditionalUpdater.MaybeUpdate<Supporter>(externalObj);

            _errorHandlerMock.Verify(errorHandler => errorHandler.HandlePullObjectFailure(externalObj, It.IsAny<Exception>()));
        }

    }
}
