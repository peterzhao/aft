using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    internal class SanityCheckerTests_NoFields_Errors
    {
        private List<String> _result;
        private string _afttosalsaqueue = "AftToSalsaQueue_donation";
        private string _afttosalsaqueueHistory = "AftToSalsaQueue_donation_History";
        private string _salsatoaftqueue = "SalsaToAftQueue_donation";
        private string _salsatoaftqueueHistory = "SalsaToAftQueue_donation_History";
        private const string ObjectType = "donation";

        [TestFixtureSetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;

            TestUtils.RemoveSyncConfigForObjectType(ObjectType);
            TestUtils.CreateSyncConfig(ObjectType, SyncDirection.Export, 1);
            TestUtils.CreateSyncConfig(ObjectType, SyncDirection.Import, 2);

            TestUtils.RemoveFieldMappingsForObjectType(ObjectType);
          
            TestUtils.CreateEmptyTable(_afttosalsaqueue);
            TestUtils.CreateEmptyTable(_afttosalsaqueueHistory);
            TestUtils.CreateEmptyTable(_salsatoaftqueue);
            TestUtils.CreateEmptyTable(_salsatoaftqueueHistory);

            var mapping1 = new FieldMapping {AftField = "f1", DataType = DataType.String, ObjectType = ObjectType, MappingRule = "aftWins", SalsaField = "f1"};
            var mapping2 = new FieldMapping {AftField = "First_Name", DataType = DataType.Int, ObjectType = ObjectType, MappingRule = "aftWins", SalsaField = "First_Name"};
            TestUtils.CreateFieldMappings(mapping1);
            TestUtils.CreateFieldMappings(mapping2);

            _result = new SanityChecker().Verify();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            TestUtils.RemoveFieldMappingsForObjectType(ObjectType);
            TestUtils.DropTable(_afttosalsaqueue);
            TestUtils.DropTable(_afttosalsaqueueHistory);
            TestUtils.DropTable(_salsatoaftqueue);
            TestUtils.DropTable(_salsatoaftqueueHistory);
            TestUtils.RemoveSyncConfigForObjectType(ObjectType);
        }



        [Test]
        public void ShouldGetErrorWhenFieldTypesDonotMatch()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "f1", DataType.String, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "f1", DataType.String, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "f1", DataType.String, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "f1", DataType.String, _afttosalsaqueueHistory), _result);
        }

        [Test]
        public void ShouldGetErrorWhenFieldIsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "First_Name", DataType.Int, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "First_Name", DataType.Int, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "First_Name", DataType.Int, _salsatoaftqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "First_Name", DataType.Int, _salsatoaftqueueHistory), _result);
        }

        [Test] 
        public void ShouldGetErrorWhenCDateIsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "CDate", DataType.DateTime, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "CDate", DataType.DateTime, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "CDate", DataType.DateTime, _salsatoaftqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "CDate", DataType.DateTime, _salsatoaftqueueHistory), _result);
        }

        [Test]
        public void ShouldGetErrorWhenSalsaKeyIsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "SalsaKey", DataType.Int, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "SalsaKey", DataType.Int, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "SalsaKey", DataType.Int, _salsatoaftqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "SalsaKey", DataType.Int, _salsatoaftqueueHistory), _result);
        }

        [Test]
        public void ShouldGetErrorWhenProcessedDateIsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "ProcessedDate", DataType.DateTime, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "ProcessedDate", DataType.DateTime, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "ProcessedDate", DataType.DateTime, _salsatoaftqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "ProcessedDate", DataType.DateTime, _salsatoaftqueueHistory), _result);
        }

        [Test]
        public void ShouldGetErrorWhenStatusIsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Status", DataType.String, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Status", DataType.String, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Status", DataType.String, _salsatoaftqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Status", DataType.String, _salsatoaftqueueHistory), _result);
        }

        [Test]
        public void ShouldGetErrorWhenIdIsMissing()
        {
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Id", DataType.Int, _afttosalsaqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Id", DataType.Int, _afttosalsaqueueHistory), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Id", DataType.Int, _salsatoaftqueue), _result);
            Assert.Contains(String.Format("Could not find column {0} as {1} in queue {2}", "Id", DataType.Int, _salsatoaftqueueHistory), _result);
        }
    }
}