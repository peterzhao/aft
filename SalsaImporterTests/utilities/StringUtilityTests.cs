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

        [Test]
        public void SpecificValueCanBeIgnored()
        {
            Assert.IsTrue(StringUtility.EqualsIncludingNullAndSpecifiedvalue("0000", "", "0000"));
            Assert.IsTrue(StringUtility.EqualsIncludingNullAndSpecifiedvalue("0000", null, "0000"));
            Assert.IsTrue(StringUtility.EqualsIncludingNullAndSpecifiedvalue("0", "", "0"));
            Assert.IsTrue(StringUtility.EqualsIncludingNullAndSpecifiedvalue("0", null, "0"));

            Assert.IsFalse(StringUtility.EqualsIncludingNullAndSpecifiedvalue("1", null, "0"));
            Assert.IsFalse(StringUtility.EqualsIncludingNullAndSpecifiedvalue("1", "", "0"));
        }
    }
}
