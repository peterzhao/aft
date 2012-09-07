using System;
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
        private SyncObject _chapterOne;
        private SyncObject _chapterTwo;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            TestUtils.ClearAllSessions();
            TestUtils.RemoveAllSalsa("custom_column", false);
            TestUtils.RemoveAllSalsa("supporter");
            TestUtils.RemoveAllSalsa("chapter");
            TestUtils.ClearAllQueues();

            TestUtils.EnsureSupporterCustomColumn("CustomDateTime0", "datetime");

            _supporterOne = MakeSupporter("One");
            _supporterTwo = MakeSupporter("Two");

            _chapterOne = MakeChapter("One");
            _chapterTwo = MakeChapter("Two");

            FieldMappingForTests.CreateInDatabase();
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

        private SyncObject MakeChapter(string arg0)
        {
            var chapter = new SyncObject("chapter");
            chapter["Name"] = string.Format("chapter{0}", arg0);
            return chapter;
        }

        [Test]
        public void ShouldImportSupporters()
        {
            TestUtils.InsertToSalsa(_supporterOne, _supporterTwo);

            new Sync().Run();

            var queue = TestUtils.ReadAllFromQueue("SalsaToAftQueue_Supporter");

            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterOne["Email"])));
            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterTwo["Email"])));

            Assert.IsTrue(queue.Any(d => d["CustomDateTime0"].Equals(_supporterTwo["CustomDateTime0"])));
            Assert.IsTrue(queue.Any(d => d["CustomDateTime0"].Equals(_supporterOne["CustomDateTime0"])));
        }

        [Test]
        public void ShouldExportSupporters()
        {
            var emailOne = "foo1@abc.com";
            var firstOne = "boo1";
            var lastOne = "joo1";
            var dateTimeOne = new DateTime(2012, 08, 29, 12, 34, 56, 00);
            TestUtils.InsertSupporterToExportQueue(emailOne, firstOne, lastOne, dateTimeOne);

            var emailTwo = "foo2@abc.com";
            var firstTwo = "boo2";
            var lastTwo = "joo2";
            var dateTimeTwo = new DateTime(2012, 08, 29, 01, 23, 45, 00);
            TestUtils.InsertSupporterToExportQueue(emailTwo, firstTwo, lastTwo, dateTimeTwo);

            new Sync().Run();

            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(2, supportersOnSalsa.Count);
            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("Email") == emailOne &&
                                                     s.StringValueOrNull("First_Name") == firstOne && 
                                                     s.StringValueOrNull("Last_Name") == lastOne && 
                                                     s.DateTimeValueOrNull("CustomDateTime0") == dateTimeOne));

            Assert.IsTrue(supportersOnSalsa.Any(s => s.StringValueOrNull("Email") == emailTwo &&
                                         s.StringValueOrNull("First_Name") == firstTwo &&
                                         s.StringValueOrNull("Last_Name") == lastTwo &&
                                         s.DateTimeValueOrNull("CustomDateTime0") == dateTimeTwo));

        }


        [Test]
        public void ShouldExportSupporterWithChapter()
        {
            // Setup
            TestUtils.InsertToSalsa(_chapterOne);
            var chapterKey = _chapterOne.SalsaKey;

            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00), chapterKey);
          
            // Test
            new Sync().Run();

            // Verify
            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(1, supportersOnSalsa.Count);

            var supporterKey = supportersOnSalsa.First().IntValueOrNull("supporter_KEY");

            List<XElement> supporterChaptersOnSalsa = TestUtils.GetAllFromSalsa("supporter_chapter");
            
            Assert.IsTrue(supporterChaptersOnSalsa.Any(supporterChapter => supporterChapter.IntValueOrDefault("supporter_KEY") == supporterKey &&
                                                                           supporterChapter.IntValueOrDefault("chapter_KEY") == chapterKey));
        }

        [Test]
        public void ShouldExportSupporterToUpdateWithNewChapter()
        {
            // Setup
            TestUtils.InsertToSalsa(_chapterOne, _chapterTwo);
           
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00), _chapterOne.SalsaKey);
            new Sync().Run();
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00), _chapterTwo.SalsaKey);

            // Test
            new Sync().Run();

            // Verify
            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(1, supportersOnSalsa.Count);

            var supporterKey = supportersOnSalsa.First().IntValueOrNull("supporter_KEY");

            List<XElement> supporterChaptersOnSalsa = TestUtils.GetAllFromSalsa("supporter_chapter");

            Assert.IsTrue(supporterChaptersOnSalsa.Any(supporterChapter => supporterChapter.IntValueOrDefault("supporter_KEY") == supporterKey &&
                                                                           supporterChapter.IntValueOrDefault("chapter_KEY") == _chapterOne.SalsaKey));

            Assert.IsTrue(supporterChaptersOnSalsa.Any(supporterChapter => supporterChapter.IntValueOrDefault("supporter_KEY") == supporterKey &&
                                                                           supporterChapter.IntValueOrDefault("chapter_KEY") == _chapterTwo.SalsaKey));
        }

        

    }
}
