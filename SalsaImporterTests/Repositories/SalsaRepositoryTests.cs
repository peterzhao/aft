using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class SalsaRepositoryTests
    {
        private const string ObjectType = "supporter";
        private SalsaRepository _repository;
        private Mock<ISalsaClient> _salsaMock;
        private Mock<IMapperFactory> _mapperFactoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISyncErrorHandler> _errorHandlerMock;
        private SyncEventArgs syncEventArgs;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            
            _salsaMock = new Mock<ISalsaClient>();
            _mapperMock = new Mock<IMapper>();
            _errorHandlerMock = new Mock<ISyncErrorHandler>();
            
            _mapperFactoryMock = new Mock<IMapperFactory>();
            _mapperFactoryMock.Setup(f => f.GetMapper(ObjectType)).Returns(_mapperMock.Object);

            _repository = new SalsaRepository(_salsaMock.Object, _mapperFactoryMock.Object, _errorHandlerMock.Object);

            _repository.NotifySyncEvent += (sender, args) => syncEventArgs = args;
        }

//        [Test]
//        public void ShouldGetObject()
//        {
//            var key = 1234;
//            var supporter = new Supporter{Email = "boo@abc.com"};
//            var xElement = XElement.Parse("<item/>");
//
//            _salsaMock.Setup(s => s.GetObject("supporter", key.ToString())).Returns(xElement);
//            _mapperMock.Setup(m => m.ToObject(xElement)).Returns(supporter);
//            _mapperMock.Setup(m => m.SalsaType).Returns("supporter");
//
//            Assert.AreEqual(supporter, _repository.Get<ISyncObject>(key));
//        }
//
//
        [Test]
        public void ShouldGetBatchOfObjects()
        {
            var syncObject = new SyncObject(ObjectType) { Id = 123 }; 
            var xElement = XElement.Parse("<item/>");
            var xElements = new List<XElement> {xElement};
            var dateTime = new DateTime(2012, 7, 20);

            _salsaMock.Setup(s => s.GetObjects(ObjectType, 10, "200", dateTime, null)).Returns(xElements);
            _mapperMock.Setup(m => m.ToObject(xElement)).Returns(syncObject);

            Assert.AreEqual(syncObject, _repository.GetBatchOfObjects(ObjectType, 10, 200, dateTime).First());
        }

        [Test]
        public void ShouldSkipBadDataInBatchFromSalsa()
        {
            var syncObject = new SyncObject(ObjectType) { Id = 123 };
            var xElement = XElement.Parse("<item/>");
            var xElements = new List<XElement> { xElement, xElement };
            var dateTime = new DateTime(2012, 7, 20);

            _salsaMock.Setup(s => s.GetObjects(ObjectType, 10, "200", dateTime, null)).Returns(xElements);

            _mapperMock.SetupSequence(m => m.ToObject(xElement))
               .Returns(syncObject)
               .Throws(new FormatException("test exception"));

            IEnumerable<SyncObject> batchOfObjects = _repository.GetBatchOfObjects(ObjectType, 10, 200, dateTime).ToList();
            Assert.AreEqual(1, batchOfObjects.Count());
            Assert.AreEqual(syncObject, batchOfObjects.First());
        }

        [Test]
        public void ShouldHandleMappingFailureInBatchFromSalsa()
        {
            var expectedException = new FormatException("test exception");
            var xUnmappableElement = XElement.Parse("<item/>");
            var xElements = new List<XElement> {  xUnmappableElement };
            var dateTime = new DateTime(2012, 7, 20);

            _salsaMock.Setup(s => s.GetObjects("supporter", 10, "200", dateTime, null)).Returns(xElements);

            _mapperMock.Setup(m => m.ToObject(xUnmappableElement))
               .Throws(expectedException);

            _repository.GetBatchOfObjects(ObjectType, 10, 200, dateTime);

            _errorHandlerMock.Verify(handler => 
                handler.HandleMappingFailure(ObjectType, xUnmappableElement, _repository, expectedException));
        }

        [Test]
        public void ShouldCreateObject()
        {
            var key = 1234;
            var id = 7890;
            var supporter = new SyncObject(ObjectType) {Id = id};
            supporter["Email"] = "foo@abc.com";
            var nameValues = new NameValueCollection();
            _mapperMock.Setup(m => m.ToNameValues(supporter)).Returns(nameValues);
            _salsaMock.Setup(s => s.Save("supporter", nameValues)).Returns(key.ToString);


             _repository.Save(supporter);
            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(_repository, syncEventArgs.Destination);
            Assert.AreEqual(SyncEventType.Export, syncEventArgs.EventType);
            Assert.AreEqual(key, syncEventArgs.ObjectId);
            Assert.AreEqual(supporter.ToString(), syncEventArgs.Data);
            
        }

        [Test]
        public void ShouldHandleExceptionWhenSavingObject()
        {
            var key = 1234;
            var id = 7890;
            var supporter = new SyncObject(ObjectType) { Id = id };
            supporter["Email"] = "foo@abc.com";
            var nameValues = new NameValueCollection();
            _mapperMock.Setup(m => m.ToNameValues(supporter)).Returns(nameValues);
            var error = new Exception("test error");
            _salsaMock.Setup(s => s.Save("supporter", nameValues)).Throws(error);


            _repository.Save(supporter);

           _errorHandlerMock.Verify(errorHandler => errorHandler.HandleSyncObjectFailure(supporter, _repository, error));

        }

      
//
//        [Test]
//        public void ShouldReturnSalsaClientCurrentTime()
//        {
//            DateTime expectedDateTime = DateTime.Now;
//            _salsaMock.SetupGet(s => s.CurrentTime).Returns(expectedDateTime);
//            Assert.AreEqual(expectedDateTime, _repository.CurrentTime);
//        }
//
//        [Test]
//        [Category("IntegrationTest")]
//        public void ShouldSaveAndGetSupporter()
//        {
//            new CreateTestingCustomColumns().CreateCustomColumns(new List<SupporterCustomColumnsRequest>()
//                                                                     {
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "String", HowManyToMake = 10, SalsaType = "varchar"},
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "Boolean", HowManyToMake = 10, SalsaType = "bool"},
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "Integer", HowManyToMake = 5, SalsaType = "int"},
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "DateTime", HowManyToMake = 1, SalsaType = "datetime"}
//                                                                     });
//
//            var name = Guid.NewGuid().ToString().Substring(0, 6);
//            var supporter = new Supporter
//                                {
//                                    First_Name = name,
//                                    Last_Name = "SalsaRepoTest",
//                                    Email = name + "@abc.com",
//                                    Organization = " LCBO  ", //salsa will trim 
//                                    CustomDateTime0 = new DateTime(2007, 3, 11, 1, 21, 17, 137), //million second will be ignored by salsa; 
//                                };
//            var repository = new SalsaRepository(new SalsaClient(), new MapperFactory(), new SyncErrorHandler(10));
//
//
//            var supporterId = repository.Add(supporter);
//            var externalSupporter = repository.Get<ISyncObject>(supporterId);
//            Assert.AreEqual(supporter, externalSupporter);
//        }
//
//        [Test]
//        [Category("IntegrationTest")]
//        public void ShouldSaveAndGetSupporterWithDayLightSavingAdjustment()
//        {
//            new CreateTestingCustomColumns().CreateCustomColumns(new List<SupporterCustomColumnsRequest>()
//                                                                     {
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "String", HowManyToMake = 10, SalsaType = "varchar"},
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "Boolean", HowManyToMake = 10, SalsaType = "bool"},
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "Integer", HowManyToMake = 5, SalsaType = "int"},
//                                                                         new SupporterCustomColumnsRequest
//                                                                             {CustomColumnName = "DateTime", HowManyToMake = 1, SalsaType = "datetime"}
//                                                                     });
//
//            var name = Guid.NewGuid().ToString().Substring(0, 6);
//            var supporter = new Supporter
//            {
//                First_Name = name,
//                Last_Name = "SalsaRepoTest",
//                Email = name + "@abc.com",
//                CustomDateTime0 = new DateTime(2007, 3, 11, 2, 21, 0), //2007/3/11 2:00 - 2:59 does not exist, will be convert to 3:00-3:59
//            };
//            var repository = new SalsaRepository(new SalsaClient(), new MapperFactory(), new SyncErrorHandler(10));
//
//
//            var supporterId = repository.Add(supporter);
//            var externalSupporter = repository.Get<ISyncObject>(supporterId);
//
//            Assert.AreNotEqual(supporter, externalSupporter);
//            Assert.IsNotNull(externalSupporter.CustomDateTime0);
//            Assert.AreEqual(3, externalSupporter.CustomDateTime0.Value.Hour);
//        }
//
//

     

    }
}
