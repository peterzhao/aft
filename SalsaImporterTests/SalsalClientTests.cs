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
            client.AuthenticateUser();
        }
        [Test]
        public void ShouldGetSupporters()
        {
            var response = client.PullObejcts();
            Assert.IsNotNullOrEmpty(response);
        }
    }
}