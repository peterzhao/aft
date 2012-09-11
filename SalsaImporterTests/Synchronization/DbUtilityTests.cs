using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class DbUtilityTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }

        [Test]
        public void ShouldGetColumnsInfo()
        {
            var info = DbUtility.GetColumnsInfo("importerLogs");
            Assert.IsTrue(info.Any(i => i.ColumnName == "time_stamp" && i.DataType.Name == "DateTime"));
            Assert.IsTrue(info.Any(i => i.ColumnName == "level" && i.DataType.Name == "String"));
        }

        [Test]
        public void ShouldCheckIfTableExist()
        {
            Assert.IsTrue(DbUtility.IsTableExist("importerLogs"));
            Assert.IsFalse(DbUtility.IsTableExist("somethingNotExist"));

        }
    }
}
