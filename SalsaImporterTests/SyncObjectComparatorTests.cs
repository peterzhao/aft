using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests
{
    [TestFixture]
    class SyncObjectComparatorTests
    {
        private List<FieldMapping> _mappings;
        private static string ObjectType = "supporter";
        private Mock<ISalsaClient> _salsaClient;
        private SyncObjectComparator _comparator;


        [SetUp]
        public void SetUp()
        {
            _salsaClient = new Mock<ISalsaClient>();
            _mappings = new List<FieldMapping>
            {
                new FieldMapping{AftField = "Email", SalsaField = "email", DataType = "string", MappingRule = MappingRules.primaryKey},
                new FieldMapping{AftField = "Address", SalsaField = "address", DataType = "string", MappingRule = MappingRules.onlyIfBlank},
                new FieldMapping{AftField = "State", SalsaField = "state", DataType = "string", MappingRule = MappingRules.onlyIfBlank},
                new FieldMapping{AftField = "NickName", SalsaField = "nick_name", DataType = "string", MappingRule = MappingRules.salsaWins},
                new FieldMapping{AftField = "SalsaLastModified", SalsaField = "LastModified", DataType = "datetime", MappingRule = MappingRules.readOnly},
                new FieldMapping{AftField = "CustomDate1", SalsaField = "custom_date1", DataType = "dateTime", MappingRule = MappingRules.aftWins},
                new FieldMapping{AftField = "ChapterKey", SalsaField = "chapter_KEY", DataType = "int", MappingRule = MappingRules.writeOnlyNewMembership},
                new FieldMapping{AftField = "GroupKey", SalsaField = "group_KEY", DataType = "int", MappingRule = MappingRules.writeOnly},
            };
            _comparator = new SyncObjectComparator(_salsaClient.Object);
        }

        [Test]
        public void ShouldBeIdenticalIgnoringFieldsWithSalsaWinsReadOnlyWriteOnlyAndPrimaryKeyRules()
        {
            var aftObject = new SyncObject(ObjectType);
            aftObject["Email"] = "abc@cde.com";
            aftObject["NickName"] = "Doo";
            aftObject["SalsaLastModified"] = new DateTime(2012, 8, 23);
            aftObject["GroupKey"] = 20;

            var salsaObject = new SyncObject(ObjectType);
            salsaObject["Email"] = "abc2@cde.com";
            salsaObject["NickName"] = "Boo";
            salsaObject["SalsaLastModified"] = new DateTime(2011, 9, 17);
            salsaObject["GroupKey"] = 98;

            Assert.IsTrue(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }

        [Test]
        public void ShouldNotBeIdenticalIfAftWinsFieldsHaveDifferentValue()
        {
            var aftObject = new SyncObject(ObjectType);
            aftObject["CustomDate1"] = new DateTime(2012, 8, 23);


            var salsaObject = new SyncObject(ObjectType);
            salsaObject["CustomDate1"] = new DateTime(2011, 9, 17);

            Assert.IsFalse(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }

        [Test]
        public void ShouldBeIdenticalIfMembershipExists()
        {
            var chapterKey = 1234;
            var supporterKey = 5678;
            var aftObject = new SyncObject(ObjectType);
            aftObject["ChapterKey"] = chapterKey;


            var salsaObject = new SyncObject(ObjectType) { SalsaKey = supporterKey };

            _salsaClient.Setup(s => s.DoesMembershipExist("chapter", ObjectType, chapterKey.ToString(), supporterKey.ToString()))
                .Returns(true);

            Assert.IsTrue(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }

        [Test]
        public void ShouldNotBeIdenticalIfMembershipNotExists()
        {
            var chapterKey = 1234;
            var supporterKey = 5678;
            var aftObject = new SyncObject(ObjectType) { SalsaKey = supporterKey };
            aftObject["ChapterKey"] = chapterKey;


            var salsaObject = new SyncObject(ObjectType);

            _salsaClient.Setup(s => s.DoesMembershipExist("chapter", ObjectType, chapterKey.ToString(), supporterKey.ToString()))
                .Returns(false);

            Assert.IsFalse(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }



        [Test]
        public void ShouldBeIdenticalIfGroupIdIs0()
        {
            var chapterKey = 0;
            var supporterKey = 5678;
            var aftObject = new SyncObject(ObjectType) { SalsaKey = supporterKey };
            aftObject["ChapterKey"] = chapterKey;

            var salsaObject = new SyncObject(ObjectType);
            Assert.IsTrue(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }




        [Test]
        public void ShouldBeIdenticalIfAftWinsFieldsHaveSameValue()
        {
            var aftObject = new SyncObject(ObjectType);
            aftObject["CustomDate1"] = new DateTime(2012, 8, 23);


            var salsaObject = new SyncObject(ObjectType);
            salsaObject["CustomDate1"] = new DateTime(2012, 8, 23);

            Assert.IsTrue(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }



        [Test]
        public void ShouldNotBeIdenticalIfOnlyIfBlankFieldIsBlankFromSalsaButNotBlankFromAft()
        {
            var aftObject = new SyncObject(ObjectType);
            aftObject["Address"] = "Girk";


            var salsaObject = new SyncObject(ObjectType);
            salsaObject["Address"] = null;

            Assert.IsFalse(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }

        [Test]
        public void ShouldBeIdenticalIfOnlyIfBlankFieldIsNotBlankFromSalsa()
        {
            var aftObject = new SyncObject(ObjectType);
            aftObject["Address"] = "Girk";


            var salsaObject = new SyncObject(ObjectType);
            salsaObject["Address"] = "Duke";

            Assert.IsTrue(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }

        [Test]
        public void ShouldBeIdenticalIfOnlyIfBlankFieldIsBlankFromSalsaButAftHasTheSameValue()
        {
            var aftObject = new SyncObject(ObjectType);
            aftObject["Address"] = null;


            var salsaObject = new SyncObject(ObjectType);
            salsaObject["Address"] = null;

            Assert.IsTrue(_comparator.AreIdentical(aftObject, salsaObject, _mappings));
        }

        [Test]
        public void ShouldNotBeIdenticalWhenSalsaObjectIsNull()
        {
            var aftObject = new SyncObject(ObjectType);
            Assert.IsFalse(_comparator.AreIdentical(aftObject, null, _mappings));
        }

        
    }
}
