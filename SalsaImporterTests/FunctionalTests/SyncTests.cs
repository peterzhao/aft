using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Repositories;
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
            TestUtils.RemoveAllSalsa("supporter_chapter");
            TestUtils.RemoveAllSalsa("supporter");
            TestUtils.RemoveAllSalsa("chapter");

            TestUtils.ClearAllQueues();

            TestUtils.EnsureSupporterCustomColumn("cdb_match_date", "datetime");

            _supporterOne = MakeSupporter("One");
            _supporterTwo = MakeSupporter("Two");

            _chapterOne = MakeChapter("One");
            _chapterTwo = MakeChapter("Two");

            TestUtils.RecreateFieldMappingForTest();
        }

        private SyncObject MakeSupporter(string arg0)
        {
            var supporter = new SyncObject("supporter");
            supporter["Email"] = string.Format("{0}@example.com", arg0);
            supporter["First_Name"] = string.Format("{0}First", arg0);
            supporter["Last_Name"] = string.Format("{0}Last", arg0);

            var hour = Math.Abs(arg0.GetHashCode())%24;
            var minute = Math.Abs(arg0.GetHashCode())%60;
            supporter["AFT_Match_DateTime"] = new DateTime(2012, 8, 29, hour, minute, 0, 0);
            
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

            new Sync().Start();
            
            var queue = TestUtils.ReadAllFromTable("SalsaToAftQueue_Supporter");

            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterOne["Email"])));
            Assert.IsTrue(queue.Any(d => d["Email"].Equals(_supporterTwo["Email"])));
            Assert.IsTrue(queue.Any(d => d["First_Name"].Equals(_supporterOne["First_Name"])));
            Assert.IsTrue(queue.Any(d => d["First_Name"].Equals(_supporterTwo["First_Name"])));

            Assert.IsTrue(queue.Any(d => d["AFT_Match_DateTime"].Equals(_supporterTwo["AFT_Match_DateTime"])));
            Assert.IsTrue(queue.Any(d => d["AFT_Match_DateTime"].Equals(_supporterOne["AFT_Match_DateTime"])));

            Assert.IsTrue(queue.All(d => (DateTime)d["Last_Modified"] >= startTime), "readonly field SalsaLastModified should be read from salsa");
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

            new Sync().Start();

            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(2, supportersOnSalsa.Count);
            XElement xmlOne = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == emailOne);
            XElement xmlTwo = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == emailTwo);
            Assert.IsNotNull(xmlOne);
            Assert.AreEqual(_supporterOne["First_Name"], xmlOne.StringValueOrNull("First_Name"));
            Assert.AreEqual(lastOne, xmlOne.StringValueOrNull("Last_Name"));
            Assert.AreEqual(_supporterOne["Title"], xmlOne.StringValueOrNull("Title"));
            Assert.AreEqual(dateTimeOne, xmlOne.DateTimeValueOrNull("cdb_match_date"));

            Assert.IsNotNull(xmlTwo);
            Assert.AreEqual(firstTwo, xmlTwo.StringValueOrNull("First_Name"));
            Assert.AreEqual(lastTwo, xmlTwo.StringValueOrNull("Last_Name"));
            Assert.AreEqual(titleTwo, xmlTwo.StringValueOrNull("Title"));
            Assert.AreEqual(dateTimeTwo, xmlTwo.DateTimeValueOrNull("cdb_match_date"));

            var rowsInQueue = TestUtils.ReadAllFromTable("AftToSalsaQueue_Supporter");
            Assert.AreEqual(0, rowsInQueue.Count);

            var rowsInHistory = TestUtils.ReadAllFromTable("AftToSalsaQueue_Supporter_History");
            Assert.AreEqual(2, rowsInHistory.Count);
            Assert.IsTrue(rowsInHistory.Any(r => r["Email"].Equals(emailOne)));
            Assert.IsTrue(rowsInHistory.Any(r => r["Email"].Equals(emailTwo)));
            Assert.IsTrue(rowsInHistory.All(r => r["Status"].Equals("Exported")));

            List<XElement> supporterChaptersOnSalsa = TestUtils.GetAllFromSalsa("supporter_chapter");
          
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

            new Sync().Start();

            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(2, supportersOnSalsa.Count);
            XElement xmlNew = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == emailOne);
            XElement xmlOld = supportersOnSalsa.FirstOrDefault(x => x.StringValueOrNull("Email") == _supporterOne["Email"].ToString());
            Assert.IsNotNull(xmlNew);
            Assert.AreEqual(firstOne, xmlNew.StringValueOrNull("First_Name"));
            Assert.AreEqual(lastOne, xmlNew.StringValueOrNull("Last_Name"));
            Assert.AreEqual(dateTimeOne, xmlNew.DateTimeValueOrNull("cdb_match_date"));

            Assert.IsNotNull(xmlOld);
            Assert.AreEqual(_supporterOne["First_Name"], xmlOld.StringValueOrNull("First_Name"));
            Assert.AreEqual(_supporterOne["Last_Name"], xmlOld.StringValueOrNull("Last_Name"));
            Assert.AreEqual(_supporterOne["AFT_Match_DateTime"], xmlOld.DateTimeValueOrNull("cdb_match_date"));

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
            var expectedAFT_Match_DateTime = new DateTime(2012, 08, 29, 12, 34, 56, 00);
            TestUtils.InsertSupporterToExportQueue(expectedEmailAddress, expectedFirstName, expectedLastName, expectedAFT_Match_DateTime, "", 0, chapterKey);
          
            // Test
            new Sync().Start();

            // Verify
            List<XElement> supportersOnSalsa = TestUtils.GetAllFromSalsa("supporter");
            Assert.AreEqual(1, supportersOnSalsa.Count);

            var supporter = supportersOnSalsa.First();
            var supporterKey = supporter.IntValueOrNull("supporter_KEY");

            Assert.AreEqual(expectedEmailAddress, supporter.StringValueOrNull("Email"));
            Assert.AreEqual(expectedFirstName, supporter.StringValueOrNull("First_Name"));
            Assert.AreEqual(expectedLastName, supporter.StringValueOrNull("Last_Name"));
            Assert.AreEqual(expectedAFT_Match_DateTime, supporter.DateTimeValueOrNull("cdb_match_date"));

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
            new Sync().Start();
            TestUtils.InsertSupporterToExportQueue("foo1@abc.com", "boo1", "joo1", new DateTime(2012, 08, 29, 12, 34, 56, 00), "", 0, _chapterTwo.SalsaKey);

            // Test
            new Sync().Start();

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

        [Test]
        public void ShouldNotExportSupporterAgainWhenObejctsAreIdentical()
        {
            TestUtils.InsertToSalsa(_chapterOne);

            var email = "foo1@abc.com";
            TestUtils.InsertSupporterToExportQueue(email, "peter", "zhao", 
                new DateTime(2012, 08, 29, 12, 34, 56, 00), "Mr", 0, _chapterOne.SalsaKey);
            new Sync().Start();

            var supporter1OnSalsa = TestUtils.GetAllFromSalsa("supporter").First();

            Thread.Sleep(2000);

            //only if blank and salsa wins fields and salsa key and million second of datetime will be ignored when checking identical
            TestUtils.InsertSupporterToExportQueue(email, "no matter for blank only rule", "no matter for blank only rule", 
                new DateTime(2012, 08, 29, 12, 34, 56, 78), "no matter for salsawins", 23, _chapterOne.SalsaKey);

            // Test
            new Sync().Start();

            var supporter2OnSalsa = TestUtils.GetAllFromSalsa("supporter").First();

            Assert.AreEqual(supporter1OnSalsa.Element("Last_Modified").Value, supporter2OnSalsa.Element("Last_Modified").Value);

            var rowsInHistory = TestUtils.ReadAllFromTable("AftToSalsaQueue_Supporter_History");
            Assert.AreEqual(2, rowsInHistory.Count);
            Assert.IsTrue(rowsInHistory.Any(r => r["Status"].Equals(QueueRepository.QueueStatusExported) && r["Email"].Equals(email)));
            Assert.IsTrue(rowsInHistory.Any(r => r["Status"].Equals(QueueRepository.QueueStatusSkipped) && r["Email"].Equals(email)));



        }





    }
}
