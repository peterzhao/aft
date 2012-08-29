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


        [Test]
        public void ShouldMapKeyToId()
        {
            var xElement = XElement.Parse(@"<item>
                                                <key>12345</key>
                                            </item>");

            var mapper = new Mapper("SomeObject", new Dictionary<string, string>());

            SyncObject syncObject = mapper.ToObject(xElement);

            Assert.AreEqual(12345, syncObject.Id);
        }

        [Test]
        public void ShouldSetValuesToSyncObjectsBasedOnMappingData()
        {
            var xElement = XElement.Parse(@"<item>
                                                <email>foo@abc.com</email>
                                                <address>main st</address>
                                            </item>");

            var mapping = new Dictionary<string, string> { { "Email", "email" }, { "Address", "address" } };
            var mapper = new Mapper("SomeObject", mapping);

            SyncObject syncObject = mapper.ToObject(xElement);

            Assert.AreEqual("foo@abc.com", syncObject["Email"]);
            Assert.AreEqual("main st", syncObject["Address"]);
        }

        [Test]
        public void ShouldNotSetNullToSyncObject()
        {
            var xElement = XElement.Parse(@"<item>
                                                <email>foo@abc.com</email>
                                                <city/>
                                            </item>");

            var mapping = new Dictionary<string, string> { { "Email", "email" }, { "City", "city" } };
            var mapper = new Mapper("SomeObject", mapping);

            SyncObject syncObject = mapper.ToObject(xElement);

            Assert.IsFalse(syncObject.FieldNames.Contains("City"));

        }



        [Test]
        public void ShouldSetKeyFromSyncObject()
        {
            var syncObject = new SyncObject("supporter");
            syncObject.Id = 1234;
            
            var mapping = new Dictionary<string, string>();
            var mapper = new Mapper(syncObject.ObjectType, mapping);

            var nameValues = mapper.ToNameValues(syncObject);
            
            Assert.AreEqual("1234", nameValues["key"]);
        }

        [Test]
        public void ShouldGetNameValuePairsFromSyncObject()
        {
            var syncObject = new SyncObject("supporter");
            syncObject["Email"] = "foo@abc.com";
            syncObject["FirstName"] = "boo";
            var mapping = new Dictionary<string, string> { { "Email", "email" }, { "FirstName", "firstname" } };
            var mapper = new Mapper(syncObject.ObjectType, mapping);

            var nameValues = mapper.ToNameValues(syncObject);
            Assert.AreEqual("boo", nameValues["firstname"]);
            Assert.AreEqual("foo@abc.com", nameValues["email"]);
        }

    }
}
