using NUnit.Framework;
using SalsaImporter;

namespace SalsaImporterTests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void ShouldGetConfigForDifferentEnvironments()
        {
            Config.Environment = Config.UnitTest;
            Assert.AreEqual("talexand@thoughtworks.com", Config.SalsaUserName);
            Assert.AreEqual("https://sandbox.salsalabs.com/", Config.SalsaApiUri);
            Assert.IsNotNull(Config.SalsaPassword);

            Config.Environment = Config.PerformanceTest;
            Assert.AreEqual("peter.qs.zhao@gmail.com", Config.SalsaUserName);
        }
    }
}