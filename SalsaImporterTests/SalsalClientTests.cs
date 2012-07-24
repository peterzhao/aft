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
            var count = client.Count();
            Assert.Greater(0, count);
        }
    }
}