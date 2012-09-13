using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class LogTrimmerTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }
        [Test]
        public void ShouldRemoveOlderLogs()
        {
            var oneDaysAgo = DateTime.Now.AddDays(-1).ToString();
            var twoDaysAgo = DateTime.Now.AddDays(-2).ToString();
            TestUtils.ExecuteSql(string.Format("delete from importerLogs", oneDaysAgo));
            TestUtils.ExecuteSql(string.Format("insert into importerLogs (time_stamp, level, message, threadid) values('{0}','debug','test1', 1)", oneDaysAgo));
            TestUtils.ExecuteSql(string.Format("insert into importerLogs (time_stamp, level, message, threadid) values('{0}','debug','test2', 1)", twoDaysAgo));
            new LogTrimmer().TrimLogsOlderThan(1);

            var rows = TestUtils.ReadAllFromTable("importerLogs");
            Assert.IsTrue(rows.Any(r => r["message"].Equals("test1")));
            Assert.IsFalse(rows.Any(r => r["message"].Equals("test2")));
            

        }
    }
}
