using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    public class StringUtilityTests
    {
        [Test]
        public void NullAndEmptyShouldBeEqual()
        {
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty(null, ""));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty("", ""));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty("", null));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty(null, null));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty("abc", "abc  "));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty("abc", "  abc"));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty("", " "));
            Assert.IsTrue(StringUtility.EqualsIncludingNullEmpty(null, " "));

            Assert.IsFalse(StringUtility.EqualsIncludingNullEmpty("abc", ""));
            Assert.IsFalse(StringUtility.EqualsIncludingNullEmpty(" ", "abc"));
            Assert.IsFalse(StringUtility.EqualsIncludingNullEmpty("dad", null));
        }
    }
}
