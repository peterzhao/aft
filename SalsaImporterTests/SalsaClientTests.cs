using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;

namespace SalsaImporterTests
{
    [TestFixture]
    public class SalsaClientTests
    {
        private SalsaClient client;
        [SetUp]
        public void SetUp()
        {
            client = new SalsaClient();
            client.Authenticate();
        }

        [Test]
        public void ShouldDeleteAllSupporters()
        {
            client.DeleteAllSupporters();
            Assert.AreEqual(0, client.Count());
        }

      

        [Test]
        public void ShouldGetSupporterById()
        {
            var firstName = NewName();
            var id = client.SaveSupporter(GenerateSupporter(firstName));
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
            client.SaveSupporter(GenerateSupporter());
            Assert.Greater(client.Count(), 0);
        }

        [Test]
        public void ShouldPullSupporters()
        {
            var total = client.Count();
            var limit = 500;
            var actualTimes = 0;
            Action<List<XElement>> action =  supports => actualTimes += 1;
            var expectedTimes = Math.Ceiling(total/500.0);
            client.GetSupporters(action, limit);

            Assert.AreEqual(expectedTimes, actualTimes);
        }

        [Test]
        public void ShouldCreateSupporter()
        {
            var id = client.SaveSupporter(GenerateSupporter());
            Assert.AreNotEqual(0, id);
            Assert.IsTrue(DoesSupporterExist(id));
        }

        [Test]
        public void ShouldCreateMultipleSupporters()
        {
            var supporters = new List<NameValueCollection>();
            for (var i = 0; i < 3; i++)
                supporters.Add(GenerateSupporter());
            
            client.SaveSupporters(supporters);
            
            foreach (var supporter in supporters)
                Assert.IsTrue(DoesSupporterExist(supporter.Get("supporter_KEY")));
        }

        [Test]
        public void ShouldDeleteMultipleSupporters()
        {
            var supporters = new List<NameValueCollection>();
            for (var i = 0; i < 3; i++)
                supporters.Add(GenerateSupporter());
            client.SaveSupporters(supporters);

            client.DeleteSupporters(supporters.Select(s => s.Get("supporter_KEY")));

            foreach (var supporter in supporters)
                Assert.IsFalse(DoesSupporterExist(supporter.Get("supporter_KEY")));
        }

        [Test]
        public void ShouldDeleteSupporter()
        {
            var id = client.SaveSupporter(GenerateSupporter());
            client.DeleteSupporter(id);
            Assert.IsFalse(DoesSupporterExist(id));

        }

        [Test]
        public void ShouldAllowDeletingDeletedSupporter()
        {
            var id = client.SaveSupporter(GenerateSupporter());
            client.DeleteSupporter(id);
            client.DeleteSupporter(id);
            Assert.IsFalse(DoesSupporterExist(id));
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
                           {"key", "0"},
                           {"First_Name", firstName},
                           {"Last_Name", "Testing"}
                       };
        }
    }
}