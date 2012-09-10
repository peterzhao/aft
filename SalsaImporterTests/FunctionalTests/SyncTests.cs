using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Salsa;
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
            supporter["First_Name"] = string.Format("{0}First", arg0);
            supporter["Last_Name"] = string.Format("{0}Last", arg0);

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
            var startTime = new SalsaClient().CurrentTime;
            TestUtils.InsertToSalsa(_supporterOne, _supporterTwo);

            new Sync().Run();
            
            var queue = TestUtils.ReadAllFromQueue("SalsaToAftQueue_Supporter");

            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterOne["Email"])));
            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterTwo["Email"])));
            Assert.IsTrue(queue.Any(d => d["First_Name"].Equals(_supporterOne["First_Name"])));
            Assert.IsTrue(queue.Any(d => d["First_Name"].Equals(_supporterTwo["First_Name"])));

            Assert.IsTrue(queue.Any(d => d["CustomDateTime0"].Equals(_supporterTwo["CustomDateTime0"])));
            Assert.IsTrue(queue.Any(d => d["CustomDateTime0"].Equals(_supporterOne["CustomDateTime0"])));

            Assert.IsTrue(queue.All(d => (DateTime)d["SalsaLastModified"] >= startTime), "readonly field SalsaLastModified should be read from salsa");
        }

        [Test]
        public void ShouldExportSupporters()
        {
            _supporterOne["Last_Name"] = "";
            _supporterOne["Title"] = "Mister";
            TestUtils.InsertToSalsa(_supporterOne);

            var emailOne = _supporterOne["Email"] as string; //email is primary key; should update
            var firstOne = "boo1"; //should not set to salsa since it is under onlyIsBlank rule
            var lastOne = "joo1"; //should set to salsa since it is under onlyIsBlank rule but salsa has blank last name
            var dateTimeOne = new DateTime(2012, 08, 29, 03, 34, 56, 00);//aft wins
            var titleOne = "Mr."; //should not set since it is under salsa wins rule
            TestUtils.InsertSupporterToExportQueue(emailOne, firstOne, lastOne, dateTimeOne, titleOne, 0); //update supportOne

            var emailTwo = "foo2@abc.com";
            var firstTwo = "boo2";
            var lastTwo = "joo2";
            var dateTimeTwo = new DateTime(2012, 08, 29, 01, 23, 45, 00);
            var titleTwo = "Miss";
            TestUtils.InsertSupporterToExportQueue(emailTwo, firstTwo, lastTwo, dateTimeTwo, titleTwo, 0);//insert a new supporter

            new Sync().Run();

            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(2, supportersOnSalsa.Count);
            XElement xmlOne = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == emailOne);
            XElement xmlTwo = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == emailTwo);
            Assert.IsNotNull(xmlOne);
            Assert.AreEqual(_supporterOne["First_Name"], xmlOne.StringValueOrNull("First_Name"));
            Assert.AreEqual(lastOne, xmlOne.StringValueOrNull("Last_Name"));
            Assert.AreEqual(_supporterOne["Title"], xmlOne.StringValueOrNull("Title"));
            Assert.AreEqual(dateTimeOne, xmlOne.DateTimeValueOrNull("CustomDateTime0"));

            Assert.IsNotNull(xmlTwo);
            Assert.AreEqual(firstTwo, xmlTwo.StringValueOrNull("First_Name"));
            Assert.AreEqual(lastTwo, xmlTwo.StringValueOrNull("Last_Name"));
            Assert.AreEqual(titleTwo, xmlTwo.StringValueOrNull("Title"));
            Assert.AreEqual(dateTimeTwo, xmlTwo.DateTimeValueOrNull("CustomDateTime0"));

        }

        [Test]
        public void ShouldCreateNewSupporterInSalsaWhenGivenSupporterHasDifferentEmail()
        {
            TestUtils.InsertToSalsa(_supporterOne);

            var emailOne = "foo1@abc.com";
            var firstOne = "boo1"; 
            var lastOne = "joo1"; 
            var dateTimeOne = new DateTime(2012, 08, 29, 03, 34, 56, 00);//aft wins
            var titleOne = "Mr."; 
            TestUtils.InsertSupporterToExportQueue(emailOne, firstOne, lastOne, dateTimeOne, titleOne, _supporterOne.SalsaKey); //update supportOne

            new Sync().Run();

            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(2, supportersOnSalsa.Count);
            XElement xmlNew = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == emailOne);
            XElement xmlOld = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == _supporterOne["Email"].ToString());
            Assert.IsNotNull(xmlNew);
            Assert.AreEqual(firstOne, xmlNew.StringValueOrNull("First_Name"));
            Assert.AreEqual(lastOne, xmlNew.StringValueOrNull("Last_Name"));
            Assert.AreEqual(dateTimeOne, xmlNew.DateTimeValueOrNull("CustomDateTime0"));

            Assert.IsNotNull(xmlOld);
            Assert.AreEqual(_supporterOne["First_Name"], xmlOld.StringValueOrNull("First_Name"));
            Assert.AreEqual(_supporterOne["Last_Name"], xmlOld.StringValueOrNull("Last_Name"));
            Assert.AreEqual(_supporterOne["CustomDateTime0"], xmlOld.DateTimeValueOrNull("CustomDateTime0"));

        }


        [Test]
        public void ShouldExportSupporterWithChapter()
        {
            // Setup
            TestUtils.InsertToSalsa(_chapterOne);
            var chapterKey = _chapterOne.SalsaKey;

            var expectedEmailAddress = "foo1@abc.com";
            var expectedFirstName = "boo1";
            var expectedLastName = "joo1";
            var expectedCustomDateTime0 = new DateTime(2012, 08, 29, 12, 34, 56, 00);
            TestUtils.InsertSupporterToExportQueue(expectedEmailAddress, expectedFirstName, expectedLastName, expectedCustomDateTime0, "", 0, chapterKey);
          
            // Test
            new Sync().Run();

            // Verify
            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(1, supportersOnSalsa.Count);

            var supporter = supportersOnSalsa.First();
            var supporterKey = supporter.IntValueOrNull("supporter_KEY");

            Assert.AreEqual(expectedEmailAddress, supporter.StringValueOrNull("Email"));
            Assert.AreEqual(expectedFirstName, supporter.StringValueOrNull("First_Name"));
            Assert.AreEqual(expectedLastName, supporter.StringValueOrNull("Last_Name"));
            Assert.AreEqual(expectedCustomDateTime0, supporter.DateTimeValueOrNull("CustomDateTime0"));

            List<XElement> supporterChaptersOnSalsa = TestUtils.GetAllFromSalsa("supporter_chapter");
            
            Assert.IsTrue(supporterChaptersOnSalsa.Any(supporterChapter => supporterChapter.IntValueOrDefault("supporter_KEY") == supporterKey &&
                                                                           supporterChapter.IntValueOrDefault("chapter_KEY") == chapterKey));
        }

        [Test]
        public void ShouldExportSupporterToUpdateWithNewChapter()
        {
            // Setup
            TestUtils.InsertToSalsa(_chapterOne, _chapterTwo);
           
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00), "", 0, _chapterOne.SalsaKey);
            new Sync().Run();
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00), "", 0, _chapterTwo.SalsaKey);

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
