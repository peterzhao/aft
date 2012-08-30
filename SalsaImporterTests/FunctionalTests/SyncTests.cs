using System.Collections.Generic;
using System.Linq;
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
            return supporter;
        }

        [Test]
        public void ShouldImportSupporters()
        {
            TestUtils.InsertToSalsa(_supporterOne, _supporterTwo);

            var sync = new Sync();
            sync.Run();

            var queue = TestUtils.ReadAllFromQueue("SalsaToAftQueue_Supporters");

            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterOne["Email"])));
            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterTwo["Email"])));

          
        }

        [Test]
        public void ShouldExportSupporters()
        {
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1");
            TestUtils.InsertSupporterToExportQueue("foo2@abc.com", "boo2", "joo2");

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
