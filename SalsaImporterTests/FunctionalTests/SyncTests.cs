using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Synchronization;
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
        public void ShouldPullSupportersFromSalsaToLocalQueue()
        {
            TestUtils.CreateSalsa(_supporterOne, _supporterTwo);

            var sync = new Sync();
            sync.Run();

            var queue = TestUtils.ReadAllFromQueue("Supporter_SalsaToAftQueue");

            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterOne["Email"])));
            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterTwo["Email"])));

          
        }
    }
}
