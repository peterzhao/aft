using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;

namespace SalsaImporterTests
{
    [TestFixture]
    public class SalsalClientTests
    {
        private SalsaClient client;
        [SetUp]
        public void SetUp()
        {
            client = new SalsaClient();
            client.Authenticate();
        }


        [Test]
        public void ShouldGetSupporters()
        {
            var response = client.PullObejcts();
            Assert.IsNotNullOrEmpty(response);
        }

        [Test]
        public void ShouldGetCountOfSupporters()
        {
            Assert.Greater(client.Count(), 0);
        }

        [Test]
        public void ShouldPullSupporters()
        {
            var total = client.Count();
            int actualTimes = 0;
            Action<List<XElement>> action =  supports => actualTimes += 1;
            var expectedTimes = Math.Ceiling(total/500.0);
            client.PullSupporters(action);

            Assert.AreEqual(expectedTimes, actualTimes);
        }
    }
}