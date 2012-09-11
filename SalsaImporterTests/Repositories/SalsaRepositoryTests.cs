using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Exceptions;
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
        private List<FieldMapping> _fieldMappings;
        private List<string> _expectedSalsaFields;

        [SetUp]
        public void SetUp()
        {
            _salsaMock = new Mock<ISalsaClient>();
            _mapperMock = new Mock<IMapper>();
            _errorHandlerMock = new Mock<ISyncErrorHandler>();
            
            _mapperFactoryMock = new Mock<IMapperFactory>();
            _mapperFactoryMock.Setup(f => f.GetMapper(ObjectType)).Returns(_mapperMock.Object);

            _repository = new SalsaRepository(_salsaMock.Object, _mapperFactoryMock.Object, _errorHandlerMock.Object);

            _repository.NotifySyncEvent += (sender, args) => syncEventArgs = args;

            _fieldMappings = new List<FieldMapping> { new FieldMapping { SalsaField = "Email" } };
            _expectedSalsaFields = new List<string> { "Email" };
        }

        [Test]
        public void ShouldGetObject()
        {
            var key = 1234;
            var supporter = new SyncObject(ObjectType);
            var xElement = XElement.Parse("<item><key>1234</key></item>");

            _salsaMock.Setup(s => s.GetObject(ObjectType, key.ToString())).Returns(xElement);
            _mapperMock.Setup(m => m.ToAft(xElement)).Returns(supporter);

            Assert.AreEqual(supporter, _repository.Get(ObjectType, key));
        }


        [Test]
        public void ShouldGetBatchOfObjects()
        {
            var syncObject = new SyncObject(ObjectType) { QueueId = 123 }; 
            var xElement = XElement.Parse("<item/>");
            var xElements = new List<XElement> {xElement};
            var dateTime = new DateTime(2012, 7, 20);

            _salsaMock.Setup(s => s.GetObjects(ObjectType, 10, 200, dateTime, _expectedSalsaFields)).Returns(xElements);
            _mapperMock.Setup(m => m.ToAft(xElement)).Returns(syncObject);
            _mapperMock.Setup(m => m.Mappings).Returns(_fieldMappings);

            Assert.AreEqual(syncObject, _repository.GetBatchOfObjects(ObjectType, 10, 200, dateTime).First());
        }

        [Test]
        public void ShouldSkipBadDataInBatchFromSalsa()
        {
            var syncObject = new SyncObject(ObjectType) { QueueId = 123 };
            var xElement = XElement.Parse("<item/>");
            var xElements = new List<XElement> { xElement, xElement };
            var dateTime = new DateTime(2012, 7, 20);

            _salsaMock.Setup(s => s.GetObjects(ObjectType, 10, 200, dateTime, _expectedSalsaFields)).Returns(xElements);

            _mapperMock.SetupSequence(m => m.ToAft(xElement))
               .Returns(syncObject)
               .Throws(new FormatException("test exception"));
            _mapperMock.Setup(m => m.Mappings).Returns(_fieldMappings);

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

            _salsaMock.Setup(s => s.GetObjects("supporter", 10, 200, dateTime, _expectedSalsaFields)).Returns(xElements);

            _mapperMock.Setup(m => m.ToAft(xUnmappableElement))
               .Throws(expectedException);
            _mapperMock.Setup(m => m.Mappings).Returns(_fieldMappings);

            _repository.GetBatchOfObjects(ObjectType, 10, 200, dateTime);

            _errorHandlerMock.Verify(handler => 
                handler.HandleMappingFailure(ObjectType, xUnmappableElement, _repository, expectedException));
        }

        [Test]
        public void ShouldSaveObject()
        {
            var key = 1234;
            var salsaObj = new SyncObject(ObjectType);
            var salsaXml = XElement.Parse("<item></item>");
            var primaryFieldMapping = new FieldMapping{AftField = "Email", SalsaField = "email"};

            var aftObj = new SyncObject(ObjectType) {SalsaKey = 1234};
            aftObj["Email"] = "foo@abc.com";
            var nameValues = new NameValueCollection();
            _mapperMock.SetupGet(m => m.PrimaryKeyMapping).Returns(primaryFieldMapping);
            _salsaMock.Setup(s => s.GetObjectBy(ObjectType, primaryFieldMapping.SalsaField, aftObj[primaryFieldMapping.AftField].ToString())).Returns(salsaXml);
            _mapperMock.Setup(m => m.ToAft(salsaXml)).Returns(salsaObj);
            _mapperMock.Setup(m => m.ToSalsa(aftObj, salsaObj)).Returns(nameValues);
            _salsaMock.Setup(s => s.Save(ObjectType, nameValues)).Returns(key.ToString);


             _repository.Save(aftObj);

            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(_repository, syncEventArgs.Destination);
            Assert.AreEqual(SyncEventType.Export, syncEventArgs.EventType);
            Assert.AreEqual(key, aftObj.SalsaKey);
            Assert.AreEqual(aftObj.ToString(), syncEventArgs.Data);
            
        }

        [Test]
        [ExpectedException(typeof(SaveToSalsaException))]
        public void ShouldHandleExceptionWhenSavingObject()
        {
            var id = 7890;
            var supporter = new SyncObject(ObjectType) {QueueId = id,};
            supporter["Email"] = "foo@abc.com";

            var primaryFieldMapping = new FieldMapping {AftField = "Email", SalsaField = "email"};
            _mapperMock.SetupGet(m => m.PrimaryKeyMapping).Returns(primaryFieldMapping);
            var error = new Exception("test error");
            _salsaMock.Setup(s => s.GetObjectBy(ObjectType, primaryFieldMapping.SalsaField, supporter["Email"].ToString()))
                .Throws(error);

            _repository.Save(supporter);
        }


    }
}
