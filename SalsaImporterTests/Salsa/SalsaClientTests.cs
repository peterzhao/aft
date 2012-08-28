using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Salsa
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SalsaClientTests
    {
        private SalsaClient client;

        public SalsaClientTests()
        {
            Config.Environment = Config.Test;
        }

        [SetUp]
        public void SetUp()
        {
            client = new SalsaClient();
        }

        [Test]
        public void ShouldAllowDeletingDeletedSupporter()
        {
            string id = client.Create("supporter", GenerateSupporter());
            client.DeleteObject("supporter", id);
            client.DeleteObject("supporter", id);
            Assert.IsFalse(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldCreateObject()
        {
            string objectType = "supporter";
            string id = client.Create(objectType, GenerateSupporter());
            Assert.AreNotEqual(0, id);
            XElement xml = client.GetObject(objectType, id);
            Assert.AreEqual(id, xml.Element("supporter_KEY").Value);
        }

        [Ignore("We don't need to delete the custom fields over and over again...")]
        [Test]
        public void ShouldDeleteAllCustomFields()
        {
            client.DeleteAllObjects("custom_column", 100, false);
            Assert.AreEqual(0, client.CountObjects("custom_column"));
        }

        [Test]
        public void ShouldDeleteAllSupporters()
        {
            CreateSupporters(5);
            client.DeleteAllObjects("supporter", 4, true);
            Assert.AreEqual(0, client.CountObjects("supporter"));
        }

        [Test]
        public void ShouldDeleteObject()
        {
            string id = client.Create("supporter", GenerateSupporter());
            client.DeleteObject("supporter", id);
            Assert.IsFalse(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldGetCountOfSupporters()
        {
            client.Create("supporter", GenerateSupporter());
            Assert.Greater(client.CountObjects("supporter"), 0);
        }

        [Test]
        public void ShouldGetEmptySupporterIdIsInvalid()
        {
            XElement support = client.GetObject("supporter", "-8976");
            Assert.IsFalse(support.HasElements);
        }
       
        [Test]
        public void ShouldGetObjects()
        {
            string objectType = "supporter";
            client.DeleteAllObjects(objectType, 100, true);

            Thread.Sleep(2000);
            DateTime lastPulledDate = DateTime.Now.AddDays(-1);

            var newSupporters = new List<NameValueCollection>();
            for (int i = 0; i < 5; i++)
            {
                NameValueCollection supporter = GenerateSupporter();
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

        [Test]
        public void ShouldGetSupporterById()
        {
            string firstName = NewName();
            string id = client.Create("supporter", GenerateSupporter(firstName));
            XElement support = client.GetObject("supporter", id);

            Assert.AreEqual(firstName, support.Element("First_Name").Value);
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
            client.CreateSupporterCustomColumn(customColumn);

            NameValueCollection supporter = GenerateSupporter();
            supporter.Add(name, valueOnSupporter);

            string supporterId = client.Create("supporter", supporter);
            XElement supporterFromSalsa = client.GetObject("supporter", supporterId);

            Assert.AreEqual(valueOnSupporter, supporterFromSalsa.Element(name).Value);
        }

        /*********** interface implimentation tests ******************/

        [Test]
        public void ShouldUpdateObject()
        {
            string objectType = "supporter";
            NameValueCollection supporter = GenerateSupporter();
            string oldFirstName = supporter["First_Name"];
            string oldEmail = supporter["EMail"];
            string id = client.Create(objectType, supporter);
            supporter["First_Name"] = NewName();
            supporter["Email"] = NewName() + "@abc.com";
            supporter["key"] = id;
            client.Update(objectType, supporter);
            XElement xml = client.GetObject(objectType, id);
            Assert.AreEqual(supporter["First_Name"], xml.Element("First_Name").Value);
            Assert.AreEqual(supporter["Email"], xml.Element("Email").Value);
            Assert.AreNotEqual(oldFirstName, xml.Element("First_Name").Value);
            Assert.AreNotEqual(oldEmail, xml.Element("Email").Value);
        }

        [Test]
        public void ShouldUpdateObjectWithSpecificFields()
        {
            string objectType = "supporter";
            NameValueCollection supporter = GenerateSupporter();
            string oldFirstName = supporter["First_Name"];
            string oldLastName = supporter["Last_Name"];

            string id = client.Create(objectType, supporter);

            supporter["First_Name"] = NewName();
            supporter["Last_Name"] = NewName() + " Foo";
            supporter["key"] = id;

            client.Update(objectType, supporter, new[] {"First_Name"});

            XElement xml = client.GetObject(objectType, id);
            Assert.AreEqual(supporter["First_Name"], xml.Element("First_Name").Value);
            Assert.AreNotEqual(oldFirstName, xml.Element("First_Name").Value);
            Assert.AreEqual(oldLastName, xml.Element("Last_Name").Value);
        }

        [Test]
        public void ShouldHaveAnIncrementingCurrentTime()
        {
            DateTime firstCurrentTime = client.CurrentTime;
            Thread.Sleep(1000);
            DateTime secondCurrentTime = client.CurrentTime;
            Assert.Greater(secondCurrentTime, firstCurrentTime);
        }

        [Test]
        public void ShouldBeAbleToGenerateAProperQueryString()
        {
            var actual = SalsaClient.GetQueryString("type", "name", Comparator.Equality, "kyle");
            var expected = "api/getCount.sjs?object=type&condition=name=kyle&countColumn=type_KEY";
            Assert.AreEqual(expected, actual);

            actual = SalsaClient.GetQueryString("type", null, null, null);
            expected = "api/getCount.sjs?object=type&countColumn=type_KEY";
            Assert.AreEqual(expected, actual);
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void ShouldThrowArgumentExceptionIfNoObjectTypeIsProvidedToGetQueryString()
        {
            SalsaClient.GetQueryString(null, null, null, null);
        }

        [Test]
        public void ShouldBeAbleToCountSupporterObjectsMatchingAQuery()
        {
            var name = NewName();
            var remoteId = client.Create("supporter", GenerateSupporter(name));

            var actual = client.CountObjectsMatchingQuery("supporter", "First_Name", Comparator.Equality, name);
            var expected = 1;

            Assert.AreEqual(expected, actual);

            client.DeleteObject("supporter", remoteId);
        }


        [Test]
        public void ShouldAllowRetrySpecificTimes()
        {
            int countOfCalled;
            countOfCalled = 0;
            Func<string> func = () =>
            {
                countOfCalled += 1;
                Console.WriteLine(countOfCalled);
                if (countOfCalled <= 2)
                    throw new InvalidDataException("testing error");

                return "OK";
            };
            Assert.DoesNotThrow(() => SalsaClient.Try<string, InvalidDataException>(func, 3));
        }

        [Test]
        public void ShoulOnlyAllowRetryForSpecificError()
        {
            int countOfCalled;
            countOfCalled = 0;
            Func<string> func = () =>
            {
                countOfCalled += 1;
                Console.WriteLine(countOfCalled);
                if (countOfCalled <= 2)
                    throw new InvalidDataException("testing error");

                return "OK";
            };
            Assert.Throws<InvalidDataException>(() => SalsaClient.Try<string, InvalidOperationException>(func, 3));
        }

        [Test]
        public void ShouldRethrowTheErrorAfterRetrySpecificTimesButStillGetError()
        {
            int countOfCalled;
            countOfCalled = 0;
            Func<string> func = () =>
            {
                countOfCalled += 1;
                Console.WriteLine(countOfCalled);
                if (countOfCalled <= 3)
                    throw new InvalidDataException("testing error");

                return "OK";
            };
            Assert.Throws<ApplicationException>(() => SalsaClient.Try<string, InvalidDataException>(func, 3));
        }
      
        private bool DoesSupporterExist(string id)
        {
            return client.GetObject("supporter", id).HasElements;
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

        private IEnumerable<NameValueCollection> CreateSupporters(int total)
        {
            var supporters = new List<NameValueCollection>();
            for (int i = 0; i < total; i++)
            {
                var supporter = GenerateSupporter();
                supporters.Add(supporter);
                supporter["supporter_KEY"] = client.Create("supporter", supporter);
            }
            return supporters;
        }
    }
}