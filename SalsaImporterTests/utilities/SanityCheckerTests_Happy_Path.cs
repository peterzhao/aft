using System;
using System.Collections.Generic;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SanityCheckerTests_Happy_Path
    {
        [Test]
        public void ShouldReturnEmptyListWhenEverythingIsFine()
        {
            Config.Environment = Config.Test;

            List<string> actual = new SanityChecker().Verify();
            Console.WriteLine(string.Join(",", actual));
            Assert.IsEmpty(actual);
        }
    }
}