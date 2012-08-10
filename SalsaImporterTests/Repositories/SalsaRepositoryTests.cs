using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class SalsaRepositoryTests
    {
        private SalsaRepository _repository;
        private Mock<ISalsaClient> _salsaMock;
        private Mock<IMapperFactory> _mapperFacotryMock;
        private Mock<IMapper> _mapperMock;
        private SyncEventArgs syncEventArgs;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.UnitTest;
            
            _salsaMock = new Mock<ISalsaClient>();
            _mapperFacotryMock = new Mock<IMapperFactory>();
            _mapperMock = new Mock<IMapper>();
            _repository = new SalsaRepository(_salsaMock.Object, _mapperFacotryMock.Object);
            _mapperFacotryMock.Setup(f => f.GetMapper<Supporter>()).Returns(_mapperMock.Object);

            _repository.NotifySyncEvent += (sender, args) => syncEventArgs = args;
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
            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(_repository, syncEventArgs.Destination);
            Assert.AreEqual(supporter, syncEventArgs.SyncObject);
            Assert.AreEqual(SyncEventType.Add, syncEventArgs.EventType);
        }

        [Test]
        public void ShouldUpdateObject()
        {
            var newObject = new Supporter { Email = "boo@abc.com", Phone = "4359088234"};
            var oldObject = new Supporter { Email = "boo@abc.com", Phone = "4359088235" };
            var nameValues = new NameValueCollection();

            _mapperMock.Setup(m => m.ToNameValues(newObject)).Returns(nameValues);

           _repository.Update(newObject);
           _salsaMock.Verify(s => s.Update("supporter", nameValues, null));

           Assert.IsNotNull(syncEventArgs);
           Assert.AreEqual(_repository, syncEventArgs.Destination);
           Assert.AreEqual(newObject, syncEventArgs.SyncObject);
           Assert.AreEqual(SyncEventType.Update, syncEventArgs.EventType);
        }

        [Test]
        public void ShouldReturnSalsaClientCurrentTime()
        {
            DateTime expectedDateTime = DateTime.Now;
            _salsaMock.SetupGet(s => s.CurrentTime).Returns(expectedDateTime);
            Assert.AreEqual(expectedDateTime, _repository.CurrentTime);
        }

        [Test]
        [Category("IntegrationTest")]
        public void ShouldSaveAndGetSupporter()
        {
            new CreateTestingCustomColumns().CreateCustomColumns();

            var name = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter = new Supporter
                                {
                                    First_Name = name,
                                    Last_Name = "SalsaRepoTest",
                                    Email = name + "@abc.com",
                                    Organization = " LCBO  ", //salsa will trim 
                                    CustomDateTime0 = new DateTime(2007, 3, 11, 1, 21, 17, 137), //million second will be ignored by salsa; 
                                };
            var repository = new SalsaRepository(new SalsaClient(), new MapperFactory());


            var supporterId = repository.Add(supporter);
            var externalSupporter = repository.Get<Supporter>(supporterId);
            Assert.AreEqual(supporter, externalSupporter);
        }

        [Test]
        [Category("IntegrationTest")]
        public void ShouldSaveAndGetSupporterWithDayLightSavingAdjustment()
        {
            new CreateTestingCustomColumns().CreateCustomColumns();

            var name = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter = new Supporter
            {
                First_Name = name,
                Last_Name = "SalsaRepoTest",
                Email = name + "@abc.com",
                CustomDateTime0 = new DateTime(2007, 3, 11, 2, 21, 0), //2007/3/11 2:00 - 2:59 does not exist, will be convert to 3:00-3:59
            };
            var repository = new SalsaRepository(new SalsaClient(), new MapperFactory());


            var supporterId = repository.Add(supporter);
            var externalSupporter = repository.Get<Supporter>(supporterId);

            Assert.AreNotEqual(supporter, externalSupporter);
            Assert.IsNotNull(externalSupporter.CustomDateTime0);
            Assert.AreEqual(3, externalSupporter.CustomDateTime0.Value.Hour);
        }



     

    }
}
