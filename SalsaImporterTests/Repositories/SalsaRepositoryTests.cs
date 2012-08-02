using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class SalsaRepositoryTests
    {
        private SalsaRepository _repository;
        private Mock<ISalsaClient> _salsaMock;
        private Mock<IMapperFactory> _mapperFacotryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _salsaMock = new Mock<ISalsaClient>();
            _mapperFacotryMock = new Mock<IMapperFactory>();
            _mapperMock = new Mock<IMapper>();
            _repository = new SalsaRepository(_salsaMock.Object, _mapperFacotryMock.Object);
            _mapperFacotryMock.Setup(f => f.GetMapper<Supporter>()).Returns(_mapperMock.Object);
        }

        [Test]
        public void ShouldGetObject()
        {
            var key = 1234;
            var supporter = new Supporter{Email = "boo@abc.com"};
            var xElement = XElement.Parse("<item/>");

            _salsaMock.Setup(s => s.GetObject(key.ToString(), "supporter")).Returns(xElement);
            _mapperMock.Setup(m => m.ToObject(xElement)).Returns(supporter);

            Assert.AreEqual(supporter, _repository.Get<Supporter>(key));
        }

        [Test]
        public void ShouldGetObjects()
        {
            var supporter = new Supporter {Email = "boo@abc.com"};
            var xElement = XElement.Parse("<item/>");
            var xElements = new List<XElement> {xElement};
            var dateTime = new DateTime(2012, 7, 20);

            _salsaMock.Setup(s => s.GetObjects("supporter",10, "200", dateTime, null)).Returns(xElements);
            _mapperMock.Setup(m => m.ToObject(xElement)).Returns(supporter);

            Assert.AreEqual(supporter, _repository.GetBatchOfObjects<Supporter>(10, 200, dateTime).First());
        }

        [Test]
        public void ShouldCreateObject()
        {
            var key = 1234;
            var supporter = new Supporter { Email = "boo@abc.com" };
            var nameValues = new NameValueCollection();

            _salsaMock.Setup(s => s.Create("supporter", nameValues)).Returns(key.ToString);
            _mapperMock.Setup(m => m.ToNameValues(supporter)).Returns(nameValues);

            Assert.AreEqual(key, _repository.Add(supporter));
        }

        [Test]
        public void ShouldUpdateObject()
        {
            var newObject = new Supporter { Email = "boo@abc.com", Phone = "4359088234"};
            var oldObject = new Supporter { Email = "boo@abc.com", Phone = "4359088235" };
            var nameValues = new NameValueCollection();

            _mapperMock.Setup(m => m.ToNameValues(newObject)).Returns(nameValues);

           _repository.Update(newObject, oldObject);
           _salsaMock.Verify(s => s.Update("supporter", nameValues, null));

        }
    }
}
