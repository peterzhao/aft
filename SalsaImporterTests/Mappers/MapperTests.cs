using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    public class MapperTests
    {
        private Mapper _mapper;
        private List<FieldMapping> _mappings;


        [SetUp]
        public void SetUp()
        {
            _mappings = new List<FieldMapping>
                                          {
                                              new FieldMapping{AftField = "Email", SalsaField = "email", DataType = "string"},
                                              new FieldMapping{AftField = "Address", SalsaField = "address", DataType = "string"},
                                          };
            _mapper = new Mapper("SomeObject", _mappings);
        }

        public void ShouldMapKeyToId()
        {
            var xElement = XElement.Parse(@"<item>
                                                <key>12345</key>
                                            </item>");

            SyncObject syncObject = _mapper.ToObject(xElement);

            Assert.AreEqual(12345, syncObject.Id);
        }

        [Test]
        public void ShouldSetValuesToSyncObjectsBasedOnMappingData()
        {
            var xElement = XElement.Parse(@"<item>
                                                <email>foo@abc.com</email>
                                                <address>main st</address>
                                            </item>");


            SyncObject syncObject = _mapper.ToObject(xElement);

            Assert.AreEqual("foo@abc.com", syncObject["Email"]);
            Assert.AreEqual("main st", syncObject["Address"]);
        }

        [Test]
        public void ShouldNotSetNullToSyncObject()
        {
            var xElement = XElement.Parse(@"<item>
                                                <email>foo@abc.com</email>
                                                <address/>
                                            </item>");


            SyncObject syncObject = _mapper.ToObject(xElement);

            Assert.IsFalse(syncObject.FieldNames.Contains("Address"));

        }
       

        [Test]
        public void ShouldGetNameValuePairsFromSyncObject()
        {
            var syncObject = new SyncObject("supporter");
            syncObject["Email"] = "foo@abc.com";
            syncObject["Address"] = "boo";
            syncObject["somethingShouldNotBeMapped"] = "hi";

            var nameValues = _mapper.ToNameValues(syncObject);
            Assert.AreEqual(2, nameValues.Keys.Count);
            Assert.AreEqual("boo", nameValues["address"]);
            Assert.AreEqual("foo@abc.com", nameValues["email"]);
        }

    }
}
