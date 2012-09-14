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
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    public class SanityTests_VerifySalsaFields
    {
        private Mock<ISalsaClient> _salsaClient;
        private List<String> _result;
        private SanityChecker _sanityChecker;
        private const string ObjectType = "donation";

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            _salsaClient = new Mock<ISalsaClient>();
            TestUtils.RemoveSyncConfigForObjectType(ObjectType);
            TestUtils.CreateSyncConfig(ObjectType, SyncDirection.Export, 1);
            TestUtils.CreateSyncConfig(ObjectType, SyncDirection.Import, 2);

            TestUtils.RemoveFieldMappingsForObjectType(ObjectType);
            var mapping1 = new FieldMapping { AftField = "f1", DataType = DataType.String, ObjectType = ObjectType, MappingRule = "aftWins", SalsaField = "f1" };
            var mapping2 = new FieldMapping { AftField = "First_Name", DataType = DataType.Int, ObjectType = ObjectType, MappingRule = "aftWins", SalsaField = "First_Name" };
            TestUtils.CreateFieldMappings(mapping1);
            TestUtils.CreateFieldMappings(mapping2);
            _sanityChecker = new SanityChecker(_salsaClient.Object);
        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.RemoveFieldMappingsForObjectType(ObjectType);
            TestUtils.RemoveSyncConfigForObjectType(ObjectType);
        }

        [Test]
        public void ShouldGetErrorWhenSalsaFieldNotExist()
        {
            _salsaClient.Setup(s => s.GetFieldList(It.IsAny<string>())).Returns(new List<string> { "" });
            _salsaClient.Setup(s => s.CountObjects(It.IsAny<string>())).Returns(10);

            _result = _sanityChecker.VerifySalsaFields();
            Assert.Contains(String.Format("Could not find field(s) {0} of {1} from Salsa", "f1,First_Name", ObjectType), _result);
        }

        [Test]
        public void CannotCheckIfSalsaFieldsExistWhenThereIsNoObjectsExistingForTheObjectType()
        {
            _salsaClient.Setup(s => s.CountObjects(It.IsAny<string>())).Returns(0);

            _result = _sanityChecker.VerifySalsaFields();
            Assert.IsEmpty(_result);
        }

        [Test]
        public void ShouldNotGetErrorWhenSalsaFieldsExist()
        {
            _salsaClient.Setup(s => s.CountObjects("supporter")).Returns(0);
            _salsaClient.Setup(s => s.CountObjects(ObjectType)).Returns(1);
            _salsaClient.Setup(s => s.GetFieldList(It.IsAny<string>())).Returns(new List<string> { "f1", "First_Name" });

            _result = _sanityChecker.VerifySalsaFields();
            Assert.IsEmpty(_result);
        }


    }
}
