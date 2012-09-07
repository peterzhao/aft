using System;
using System.Collections.Generic;
using System.Linq;
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
                                              new FieldMapping{AftField = "Email", SalsaField = "email", DataType = "string", MappingRule = MappingRules.aftWins},
                                              new FieldMapping{AftField = "Address", SalsaField = "address", DataType = "string", MappingRule = MappingRules.onlyIfBlank},
                                              new FieldMapping{AftField = "State", SalsaField = "state", DataType = "string", MappingRule = MappingRules.onlyIfBlank},
                                              new FieldMapping{AftField = "NickName", SalsaField = "nick_name", DataType = "string", MappingRule = MappingRules.salsaWins},
                                              new FieldMapping{AftField = "SalsaLastModified", SalsaField = "LastModified", DataType = "datetime", MappingRule = "READONLY"},
                                              new FieldMapping{AftField = "CustomDate1", SalsaField = "custom_date1", DataType = "dateTime", MappingRule = MappingRules.aftWins},
                                          };
            _mapper = new Mapper("SomeObject", _mappings);
        }

        public void ShouldMapKeyToId()
        {
            var xElement = XElement.Parse(@"<item>
                                                <key>12345</key>
                                            </item>");

            SyncObject syncObject = _mapper.ToAft(xElement);

            Assert.AreEqual(12345, syncObject.QueueId);
        }

        [Test]
        public void ShouldSetValuesToSyncObjectsBasedOnMappingData()
        {
            var xElement = XElement.Parse(@"<item>
                                                <email>foo@abc.com</email>
                                                <address>main st</address>
                                                <key>5678</key>
                                            </item>");


            SyncObject syncObject = _mapper.ToAft(xElement);

            Assert.AreEqual("foo@abc.com", syncObject["Email"]);
            Assert.AreEqual("main st", syncObject["Address"]);
            Assert.AreEqual(5678, syncObject.SalsaKey);
        }

        [Test]
        public void ShouldReturnNullSyncObjectWhenGivenXmlElementHasNoChild()
        {
            var xElement = XElement.Parse(@"<item></item>");
            SyncObject syncObject = _mapper.ToAft(xElement);
            Assert.IsNull(syncObject);
        }

        [Test]
        public void ShouldSetDateTimeFieldsToSyncObject()
        {
            var xElement = XElement.Parse(@"<item>
                                                <custom_date1>Thu Aug 30 2012 11:19:43 GMT-0400 (EDT)</custom_date1>
                                            </item>");


            SyncObject syncObject = _mapper.ToAft(xElement);

            Assert.AreEqual(new DateTime(2012, 08, 30, 11, 19, 43), syncObject["CustomDate1"]);
        }

        [Test]
        public void ShouldNotSetValueForAFieldWhenSalsaObjectHasNoneEmptyValueWhenMappingRuleIsIfOnlyBlank()
        {
            
            var salsaObject = new SyncObject("supporter");
            salsaObject["State"] = "Kansas";

            var aftObject = new SyncObject("supporter");
            aftObject["State"] = "California";
            aftObject["Address"] = "100 main";
            var nameValues = _mapper.ToSalsa(aftObject, salsaObject);

            Assert.AreEqual("100 main", nameValues["address"]);
            Assert.IsNull(nameValues["state"]);
        }

        [Test]
        public void ShouldSetValueForAFieldFromAftObjectWhenSalsaObjectNotExistWhenMappingRuleIsIfOnlyBlank()
        {
            var aftObject = new SyncObject("supporter");
            aftObject["State"] = "California";
            aftObject["Address"] = "100 main";
            var nameValues = _mapper.ToSalsa(aftObject, null);

            Assert.AreEqual("100 main", nameValues["address"]);
            Assert.AreEqual("California", nameValues["state"]);
        }

        [Test]
        public void ShouldSetValueForAFieldFromAftObjectWhenSalsaObjectNotExistWhenMappingRuleIsSalsaWins()
        {
            var aftObject = new SyncObject("supporter");
            aftObject["NickName"] = "Dan";
            var nameValues = _mapper.ToSalsa(aftObject, null);

            Assert.AreEqual("Dan", nameValues["nick_name"]);
        }

        [Test]
        public void ShouldNotSetValueForAFieldFromAftObjectWhenSalsaObjectExistsWhenMappingRuleIsSalsaWins()
        {
            var aftObject = new SyncObject("supporter");
            var salsaObject = new SyncObject("supporter");
            aftObject["NickName"] = "Dan";
            var nameValues = _mapper.ToSalsa(aftObject, salsaObject);
            Assert.IsFalse(nameValues.AllKeys.Contains("nick_name"));
            Assert.IsNull(nameValues["nick_name"]);
        }



        [Test]
        public void ShouldSkipReadOnlyFieldWhenNameValuePairs()
        {
            var syncObject = new SyncObject("supporter");
            syncObject["Email"] = "foo@abc.com";
            syncObject["SalsaLastModified"] = DateTime.Now;

            var nameValues = _mapper.ToSalsa(syncObject, null);
            Assert.IsFalse(nameValues.AllKeys.ToList().Contains("LastModified"));
        }

        [Test]
        public void ShouldNotSetNullToSyncObject()
        {
            var xElement = XElement.Parse(@"<item>
                                                <email>foo@abc.com</email>
                                                <address/>
                                            </item>");


            SyncObject syncObject = _mapper.ToAft(xElement);

            Assert.IsFalse(syncObject.FieldNames.Contains("Address"));
        }
       
        [Test]
        public void ShouldGetNameValuePairsFromSyncObjectWithoutSalsaKey()
        {
            var syncObject = new SyncObject("supporter");
            syncObject["Email"] = "foo@abc.com";
            syncObject["Address"] = "boo";
            syncObject["somethingShouldNotBeMapped"] = "hi";

            var nameValues = _mapper.ToSalsa(syncObject, null);
            Assert.AreEqual(3, nameValues.Keys.Count);
            Assert.AreEqual("boo", nameValues["address"]);
            Assert.AreEqual("foo@abc.com", nameValues["email"]);
            Assert.AreEqual("0", nameValues["key"]);
        }

        [Test]
        public void ShouldGetNameValuePairsFromSyncObjectWithSalsaKey()
        {
            var syncObject = new SyncObject("supporter");
            syncObject.SalsaKey = 4567;
            syncObject["Email"] = "foo@abc.com";

            var nameValues = _mapper.ToSalsa( syncObject, null);
            Assert.AreEqual(2, nameValues.Keys.Count);
            Assert.AreEqual("foo@abc.com", nameValues["email"]);
            Assert.AreEqual("4567", nameValues["key"]);
        }

        [Test]
        public void ShouldCreateNameValuePairsFromSyncObjectWithDateTime()
        {
            var syncObject = new SyncObject("supporter");
            syncObject["CustomDate1"] = new DateTime(2012, 08, 29, 12, 34, 56);
            
            var nameValues = _mapper.ToSalsa(syncObject, null);
            Assert.AreEqual(2, nameValues.Keys.Count);
            Assert.AreEqual("2012-08-29 12:34:56", nameValues["custom_date1"]);
        }

    }
}
