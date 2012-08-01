using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests
{
    [TestFixture]
    public class SalsaClientTests
    {
        public SalsaClientTests()
        {
            Config.Environment = Config.UnitTest;
        }

        private SalsaClient client;
        [SetUp]
        public void SetUp()
        {
            client = new SalsaClient(new SyncErrorHandler(100, 500));
        }


        public void ShouldGetSupportersEventhoughHavingNoSupporters()
        {
            client.DeleteAllObjects("supporter", 100, true);
            var actualTimes = 0;
            Action<List<XElement>> action = supports => actualTimes += 1;
            client.EachBatchOfObjects("supporter", 500, action);
            Assert.AreEqual(0, actualTimes);
        }

     

        [Test]
        public void ShouldDeleteAllSupporters()
        {
            var supporters = new List<NameValueCollection>();
            for (var i = 0; i < 10; i++)
                supporters.Add(GenerateSupporter());
            client.CreateSupporters(supporters);
            client.DeleteAllObjects("supporter",3, true);
            Assert.AreEqual(0, client.SupporterCount());
        }

        [Test]
        public void ShouldGetSupporterById()
        {
            var firstName = NewName();
            var id = client.CreateSupporter(GenerateSupporter(firstName));
            XElement support = client.GetSupporter(id);

            Assert.AreEqual(firstName, support.Element("First_Name").Value);
        }

        [Test]
        public void ShouldGetEmptySupporterIdIsInvalid()
        {
            XElement support = client.GetSupporter("-8976");
            Assert.IsFalse(support.HasElements);
        }

        [Test]
        public void ShouldGetEmptySupporterIdIsInvalidString()
        {
            XElement support = client.GetSupporter("foobar");
            Assert.IsFalse(support.HasElements);
        }

        [Test]
        public void ShouldGetCountOfSupporters()
        {
            client.CreateSupporter(GenerateSupporter());
            Assert.Greater(client.SupporterCount(), 0);
        }

        [Test]
        public void ShouldPullSupporters()
        {
            var supporters = new List<NameValueCollection>();
            for (var i = 0; i < 10; i++)
                supporters.Add(GenerateSupporter());
            client.CreateSupporters(supporters);

            var total = client.SupporterCount();
            Console.WriteLine("total:" + total);
            var limit = 3;
            var actualTimes = 0;
            Action<List<XElement>> action =  supports => actualTimes += 1;
            var expectedTimes = Math.Ceiling(total/3.0);
            client.EachBatchOfObjects("supporter", limit, action);

            Assert.AreEqual(expectedTimes, actualTimes);
        }

        [Test]
        public void ShouldCreateSupporter()
        {
            var id = client.CreateSupporter(GenerateSupporter());
            Assert.AreNotEqual(0, id);
            Assert.IsTrue(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldCreateMultipleSupporters()
        {
            var supporters = new List<NameValueCollection>();
            for (var i = 0; i < 3; i++)
                supporters.Add(GenerateSupporter());
            
            client.CreateSupporters(supporters);

            foreach (var supporter in supporters)
            {
                var id = supporter.Get("supporter_KEY");
                Assert.IsTrue(DoesSupporterExist(id));
            }
        }

        [Test]
        public void ShouldDeleteMultipleSupporters()
        {
            var supporters = new List<NameValueCollection>();
            for (var i = 0; i < 3; i++)
                supporters.Add(GenerateSupporter());
            client.CreateSupporters(supporters);

            client.DeleteObjects("supporter", supporters.Select(s => s.Get("supporter_KEY")));

            foreach (var supporter in supporters)
                Assert.IsFalse(DoesSupporterExist(supporter.Get("supporter_KEY")));
        }

        [Test]
        public void ShouldDeleteSupporter()
        {
            var id = client.CreateSupporter(GenerateSupporter());
            client.DeleteObject("supporter", id);
            Assert.IsFalse(DoesSupporterExist(id));

        }

        [Test]
        public void ShouldAllowDeletingDeletedSupporter()
        {
            var id = client.CreateSupporter(GenerateSupporter());
            client.DeleteObject("supporter", id);
            client.DeleteObject("supporter", id);
            Assert.IsFalse(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldDeleteAllCustomFields()
        {
            client.DeleteAllObjects("custom_column", 100, false);
            Assert.AreEqual(0, client.CustomColumnCount());
        }

        [Test]
        public void ShouldReadWriteCustomFieldOnSupporter()
        {
            string name = "testfield";
            string valueOnSupporter = "TestCustomFieldValue";

            var customColumn = new NameValueCollection
                                          {
                                              {"name", name},
                                              {"label", "Test Field"}, 
                                              {"type", "varchar"}
                                          };
            
            client.DeleteAllObjects("custom_column", 100, false);
            client.CreateSupporterCustomColumn(customColumn);

            NameValueCollection supporter = GenerateSupporter();
            supporter.Add(name, valueOnSupporter);

            string supporterId = client.CreateSupporter(supporter);
            XElement supporterFromSalsa = client.GetSupporter(supporterId);

            Assert.AreEqual(valueOnSupporter, supporterFromSalsa.Element(name).Value);
        }

        /*********** interface implimentation tests ******************/
        [Test]
        public void ShouldCreateObject()
        {
            var objectType = "supporter";
            var id = client.Create(objectType, GenerateSupporter());
            Assert.AreNotEqual(0, id);
            var xml = client.GetObject(id, objectType);
            Assert.AreEqual(id, xml.Element("supporter_KEY").Value);
        }

        [Test]
        public void ShouldUpdateObject()
        {
            var objectType = "supporter";
            var supporter = GenerateSupporter();
            var oldFirstName = supporter["First_Name"];
            var oldEmail = supporter["EMail"];
            var id = client.Create(objectType, supporter);
            supporter["First_Name"] = NewName();
            supporter["Email"] = NewName() + "@abc.com";
            supporter["key"] = id;
            client.Update(objectType, supporter);
            var xml = client.GetObject(id, objectType);
            Assert.AreEqual(supporter["First_Name"], xml.Element("First_Name").Value);
            Assert.AreEqual(supporter["Email"], xml.Element("Email").Value);
            Assert.AreNotEqual(oldFirstName, xml.Element("First_Name").Value);
            Assert.AreNotEqual(oldEmail, xml.Element("Email").Value);
        }

        [Test]
        public void ShouldUpdateObjectWithSpificFields()
        {
            var objectType = "supporter";
            var supporter = GenerateSupporter();
            var oldFirstName = supporter["First_Name"];
            var oldLastName = supporter["Last_Name"];

            var id = client.Create(objectType, supporter);

            supporter["First_Name"] = NewName();
            supporter["Last_Name"] = NewName() + " Foo";
            supporter["key"] = id;

            client.Update(objectType, supporter, new[] { "First_Name" });

            var xml = client.GetObject(id, objectType);
            Assert.AreEqual(supporter["First_Name"], xml.Element("First_Name").Value);
            Assert.AreNotEqual(oldFirstName, xml.Element("First_Name").Value);
            Assert.AreEqual(oldLastName, xml.Element("Last_Name").Value);
        }

        [Test]
        public void ShouldGetObjects()
        {

            var objectType = "supporter";
            client.DeleteAllObjects(objectType, 100, true);

  
            Thread.Sleep(2000);
            var lastPulledDate = DateTime.Now.AddDays(-1);

            var newSupporters = new List<NameValueCollection>();
            for (var i = 0; i < 5; i++)
            {
                var supporter = GenerateSupporter();
                newSupporters.Add(supporter);
                supporter["supporter_KEY"] = client.Create(objectType, supporter);
            }
            Thread.Sleep(2000);

            List<string> batch1 = client.GetObjects(objectType, 3, "0", lastPulledDate).Select(x => x.Element("supporter_KEY").Value).ToList();
            List<string> batch2 = client.GetObjects(objectType, 3, batch1.Last(), lastPulledDate).Select(x => x.Element("supporter_KEY").Value).ToList();
            List<string> batch3 = client.GetObjects(objectType, 3, batch2.Last(), lastPulledDate).Select(x => x.Element("supporter_KEY").Value).ToList();

            Assert.IsTrue(batch1.All(id => newSupporters.Any(nameValues => nameValues["supporter_KEY"] == id)));
            Assert.IsTrue(batch2.All(id => newSupporters.Any(nameValues => nameValues["supporter_KEY"] == id)));
            Assert.AreEqual(0, batch3.Count);
        }
       

        private bool DoesSupporterExist(string id)
        {
            return client.GetSupporter(id).HasElements;
        }

      

        private static string NewName()
        {
            return Guid.NewGuid().ToString().Substring(0, 6);
        }


        private static NameValueCollection GenerateSupporter()
        {
            return GenerateSupporter(NewName());
        }

        private static NameValueCollection GenerateSupporter(string firstName)
        {
            return new NameValueCollection
                       {
                           {"Email", firstName + "@abc.com"},
                           {"First_Name", firstName},
                           {"Last_Name", "Testing"}
                       };
        }


    }
}