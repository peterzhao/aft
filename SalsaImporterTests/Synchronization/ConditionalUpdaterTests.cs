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
            var sourceObject = new Supporter {ExternalId = 1234};
            int newlocalId = 3456;
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(sourceObject.Id)).Returns(()=> null);
            _localRepositoryMock.Setup(localRepository => localRepository.Add(sourceObject)).Returns(newlocalId);

            _conditionalUpdater.MaybeUpdate<Supporter>(sourceObject);

            _localRepositoryMock.Verify(localRepository => localRepository.Add(sourceObject));
        }

        [Test]
        public void ShouldDoNothingIfExternalIsSameAsLocal()
        {
            var externalObj = new Supporter { Id = 1234, Email = "jj@abc.com", ExternalId = 5678};
            var localObj = new Supporter { ExternalId = 1234, Email = "jj@abc.com", Id = 5678};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.Id)).Returns(localObj);

            _conditionalUpdater.MaybeUpdate<Supporter>(externalObj);

            _localRepositoryMock.Verify(localRepository => localRepository.Add(It.IsAny<Supporter>()), Times.Never());
            _localRepositoryMock.Verify(localRepository => localRepository.Update(It.IsAny<Supporter>()), Times.Never());
        }

        [Test]
        public void ShouldUpdateLocalObjectIfExternalIsDifferentThanLocal()
        {
            var externalObj = new Supporter { Id = 1234, Email = "jj@abc.com", ExternalId = 5678, Phone = "4161234567"};
            var localObj = new Supporter { ExternalId = 1234, Email = "jj@abc.com", Id = 5678, Phone = "4161234568"};
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.Id)).Returns(localObj);

            _conditionalUpdater.MaybeUpdate<Supporter>(externalObj);

            _localRepositoryMock.Verify(localRepository => localRepository.Update(externalObj));
        }

        [Test]
        public void ShouldHandleError()
        {
            var externalObj = new Supporter { ExternalId = 1234, Email = "jj@abc.com", Id = 5678 };
            _localRepositoryMock.Setup(localRepository => localRepository.GetByExternalKey<Supporter>(externalObj.Id))
                .Throws<Exception>();

            _conditionalUpdater.MaybeUpdate<Supporter>(externalObj);

            _errorHandlerMock.Verify(errorHandler => errorHandler.HandlePullObjectFailure(externalObj, It.IsAny<Exception>()));
        }

    }
}
