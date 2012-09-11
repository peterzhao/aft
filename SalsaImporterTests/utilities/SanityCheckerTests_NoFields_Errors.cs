using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    internal class SanityCheckerTests_NoFields_Errors
    {
        private List<String> _result;
        private const string ObjectType = "supporter";

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            TestUtils.RemoveFieldMappings();
            var mapping1 = new FieldMapping {AftField = "f1", DataType = DataType.String, ObjectType = ObjectType, MappingRule = "aftWins", SalsaField = "f1"};
            var mapping2 = new FieldMapping {AftField = "First_Name", DataType = DataType.Int, ObjectType = ObjectType, MappingRule = "aftWins", SalsaField = "First_Name"};
            TestUtils.CreateFieldMappings(mapping1);
            TestUtils.CreateFieldMappings(mapping2);

            _result = new SanityChecker().Verify();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.RecreateFieldMappingForTest();
        }



        [Test]
        public void ShouldGetErrorWhenFieldsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "f1", DataType.String, "AftToSalsaQueue_supporter"), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "f1", DataType.String, "AftToSalsaQueue_supporter"), _result);
        }

        [Test]
        public void ShouldGetErrorWhenFieldTypesDonotMatch()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "First_Name", DataType.Int, "AftToSalsaQueue_supporter"), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "First_Name", DataType.Int, "SalsaToAftQueue_supporter"), _result);
        }
    }
}