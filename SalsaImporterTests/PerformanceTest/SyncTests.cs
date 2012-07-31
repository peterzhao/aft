using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests.PerformanceTest
{
    [TestFixture]
    [Category("PerformanceTest")]
    public class SyncTests
    {
        public SyncTests()
        {
            Config.Environment = Config.PerformanceTest;
            //Config.Environment = Config.Stub;
        }

        [Test]
        public void ShouldPushSupportersToSalsa()
        {
           new Sync().PushNewSupportsToSalsa();
        }

    }
}