using NUnit.Framework;
using SalsaImporter;

namespace SyncPerformanceTest
{
    [TestFixture]
    [Category("PerformanceTest")]
    public class SyncTests
    {
        public SyncTests()
        {
            Config.Environment = Config.PerformanceTest;
        }

        [Test]
        public void ShouldPushSupportersToSalsa()
        {
            new Sync().Run();
        }

    }
}