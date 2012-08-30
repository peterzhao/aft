﻿using System;
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
                                              new FieldMapping{AftField = "SalsaLastModified", SalsaField = "some_date", DataType = "datetime"},
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
                                                <key>5678</key>
                                            </item>");


            SyncObject syncObject = _mapper.ToObject(xElement);

            Assert.AreEqual("foo@abc.com", syncObject["Email"]);
            Assert.AreEqual("main st", syncObject["Address"]);
            Assert.AreEqual(5678, syncObject.SalsaKey);
        }

        [Test]
        public void ShouldSetDateTimeFieldsToSyncObject()
        {
            var xElement = XElement.Parse(@"<item>
                                                <some_date>Thu Aug 30 2012 11:19:43 GMT-0400 (EDT)</some_date>
                                            </item>");


            SyncObject syncObject = _mapper.ToObject(xElement);

            Assert.AreEqual(new DateTime(2012, 08, 30, 11, 19, 43), syncObject["SalsaLastModified"]);
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
        public void ShouldGetNameValuePairsFromSyncObjectWithoutSalsaKey()
        {
            var syncObject = new SyncObject("supporter");
            syncObject["Email"] = "foo@abc.com";
            syncObject["Address"] = "boo";
            syncObject["somethingShouldNotBeMapped"] = "hi";

            var nameValues = _mapper.ToNameValues(syncObject);
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

            var nameValues = _mapper.ToNameValues(syncObject);
            Assert.AreEqual(2, nameValues.Keys.Count);
            Assert.AreEqual("foo@abc.com", nameValues["email"]);
            Assert.AreEqual("4567", nameValues["key"]);
        }

        [Test]
        public void ShouldCreateNameValuePairsFromSyncObjectWithDateTime()
        {
            var syncObject = new SyncObject(null);
            syncObject["SalsaLastModified"] = new DateTime(2012, 08, 29, 12, 34, 56);
            
            var nameValues = _mapper.ToNameValues(syncObject);
            Assert.AreEqual(2, nameValues.Keys.Count);
            Assert.AreEqual("2012-08-29 12:34:56", nameValues["some_date"]);
        }

    }
}
