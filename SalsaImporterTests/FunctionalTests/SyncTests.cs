using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.FunctionalTests
{
    [TestFixture]
    [Category("FunctionalTest")]
    public class SyncTests
    {
        private SyncObject _supporterOne;
        private SyncObject _supporterTwo;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            TestUtils.ClearAllSessions();
            TestUtils.RemoveAllSalsa("supporter");
            TestUtils.ClearAllQueues();

            _supporterOne = MakeSupporter("One");
            _supporterTwo = MakeSupporter("Two");
        }

        private SyncObject MakeSupporter(string arg0)
        {
            var supporter = new SyncObject("supporter");
            supporter["Email"] = string.Format("{0}@example.com", arg0);
            supporter["First_Name"] = "Supporter";
            supporter["Last_Name"] = string.Format("{0}", arg0);

            var hour = Math.Abs(arg0.GetHashCode())%24;
            var minute = Math.Abs(arg0.GetHashCode())%60;
            supporter["CustomDateTime0"] = new DateTime(2012, 8, 29, hour, minute, 0, 0);
            
            return supporter;
        } 

        [Test]
        public void ShouldImportSupporters()
        {
            TestUtils.InsertToSalsa(_supporterOne, _supporterTwo);
            var sync = new Sync();
            sync.Run();

            var queue = TestUtils.ReadAllFromQueue("SalsaToAftQueue_Supporter");

            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterOne["Email"])));
            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterTwo["Email"])));

            //TODO: The following lines need a FieldMapping for CustomDateTime0
            //Assert.IsTrue(queue.Any(d => d["CustomDateTime0"].Equals(_supporterTwo["CustomDateTime0"])));
            //Assert.IsTrue(queue.Any(d => d["CustomDateTime0"].Equals(_supporterOne["CustomDateTime0"])));
        }

        [Test]
        public void ShouldExportSupporters()
        {
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00));
            TestUtils.InsertSupporterToExportQueue("foo2@abc.com", "boo2", "joo2", new DateTime(2012, 08, 29, 01, 23, 45, 00));

            var sync = new Sync();
            sync.Run();

            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(2, supportersOnSalsa.Count);
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("Email") == "foo1@abc.com"));
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("First_Name") == "boo1"));
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("Last_Name") == "joo1"));
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("Email") == "foo2@abc.com"));
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("First_Name") == "boo2"));
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("Last_Name") == "joo2"));
        }
    }
}
