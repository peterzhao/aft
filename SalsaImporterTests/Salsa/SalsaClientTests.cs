using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Exceptions;
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
            string id = client.Save("supporter", GenerateSupporter());
            client.DeleteObject("supporter", id);
            client.DeleteObject("supporter", id);
            Assert.IsFalse(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldCreateObject()
        {
            string objectType = "supporter";
            string id = client.Save(objectType, GenerateSupporter());
            Assert.AreNotEqual(0, id);
            XElement xml = client.GetObject(objectType, id);
            Assert.AreEqual(id, xml.Element("supporter_KEY").Value);
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
            string id = client.Save("supporter", GenerateSupporter());
            client.DeleteObject("supporter", id);
            Assert.IsFalse(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldGetCountOfSupporters()
        {
            client.Save("supporter", GenerateSupporter());
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
                supporter["supporter_KEY"] = client.Save(objectType, supporter);
            }
            Thread.Sleep(2000);

            List<int> batch1 = client.GetObjects(objectType, 3, 0, lastPulledDate).Select(x => int.Parse(x.Element("supporter_KEY").Value)).ToList();
            List<int> batch2 = client.GetObjects(objectType, 3, batch1.Last(), lastPulledDate).Select(x => int.Parse(x.Element("supporter_KEY").Value)).ToList();
            List<int> batch3 = client.GetObjects(objectType, 3, batch2.Last(), lastPulledDate).Select(x => int.Parse(x.Element("supporter_KEY").Value)).ToList();

            Assert.IsTrue(batch1.All(id => newSupporters.Any(nameValues => int.Parse(nameValues["supporter_KEY"]) == id)));
            Assert.IsTrue(batch2.All(id => newSupporters.Any(nameValues => int.Parse(nameValues["supporter_KEY"]) == id)));
            Assert.AreEqual(0, batch3.Count);
        }

        [ExpectedException(typeof(ApplicationException))]
        [Test]
        public void ShouldDetectInvalidFieldsWhenGettingObjects()
        {
            string objectType = "supporter";
            client.GetObjects(objectType, 3, 0, new DateTime(1991, 1, 1), new List<string> {"InvalidFieldName"});
        }

        [Test]
        public void ShouldGetSupporterById()
        {
            string firstName = NewName();
            string id = client.Save("supporter", GenerateSupporter(firstName));
            XElement support = client.GetObject("supporter", id);

            Assert.AreEqual(firstName, support.Element("First_Name").Value);
        }

        [Test]
        public void ShouldGetSupporterByPrimaryKey()
        {
            string firstName = NewName();
            var supporterData = GenerateSupporter(firstName);
            string id = client.Save("supporter", supporterData);
            XElement support = client.GetObjectBy("supporter","Email", supporterData["Email"]);

            Assert.AreEqual(firstName, support.Element("First_Name").Value);
            Assert.AreEqual(supporterData["Email"], support.Element("Email").Value);
        }

        [Test]
        public void ShouldGetSupporterByKey()
        {
            string firstName = NewName();
            var supporterData = GenerateSupporter(firstName);
            string id = client.Save("supporter", supporterData);
            XElement support = client.GetObjectBy("supporter", "key", id);

            Assert.AreEqual(firstName, support.Element("First_Name").Value);
            Assert.AreEqual(id, support.Element("key").Value);
        }

        [Test]
        public void ShouldGetEmptyXmlElementIfNoObjectCanBeFoundByPrimaryKey()
        {
            XElement support = client.GetObjectBy("supporter", "Email", "someNotExist");

            Assert.IsFalse(support.HasElements);
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

            string supporterId = client.Save("supporter", supporter);
            XElement supporterFromSalsa = client.GetObject("supporter", supporterId);

            Assert.AreEqual(valueOnSupporter, supporterFromSalsa.Element(name).Value);
        }

        /*********** interface implementation tests ******************/

        [Test]
        public void ShouldUpdateObject()
        {
            string objectType = "supporter";
            NameValueCollection supporter = GenerateSupporter();
            string oldFirstName = supporter["First_Name"];
            string oldEmail = supporter["EMail"];
            string id = client.Save(objectType, supporter);
            supporter["First_Name"] = NewName();
            supporter["Email"] = NewName() + "@abc.com";
            supporter["key"] = id;
            client.Save(objectType, supporter);
            XElement xml = client.GetObject(objectType, id);
            Assert.AreEqual(supporter["First_Name"], xml.Element("First_Name").Value);
            Assert.AreEqual(supporter["Email"], xml.Element("Email").Value);
            Assert.AreNotEqual(oldFirstName, xml.Element("First_Name").Value);
            Assert.AreNotEqual(oldEmail, xml.Element("Email").Value);
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
            var remoteId = client.Save("supporter", GenerateSupporter(name));

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
                supporter["supporter_KEY"] = client.Save("supporter", supporter);
            }
            return supporters;
        }
    }
}