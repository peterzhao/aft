using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    public class DateTimeUtilityTests
    {
        [Test]
        public void ShouldCompareDateTimesIgnoringMillionSeconds()
        {
            DateTime? dt1 =  new DateTime(2012, 7, 15, 9, 30, 27,15);
            DateTime? dt2 =  new DateTime(2012, 7, 15, 9, 30, 27,35);
            DateTime? dt3 =  new DateTime(2012, 7, 15, 9, 31, 27,35);
            DateTime? dt4 =  null;
            DateTime? dt5 =  null;

            Assert.IsTrue(dt4.EqualsIgnoreMillionSeconds(dt5));
            Assert.IsTrue(dt1.EqualsIgnoreMillionSeconds(dt2));
            Assert.IsTrue(dt2.EqualsIgnoreMillionSeconds(dt1));
            Assert.IsFalse(dt2.EqualsIgnoreMillionSeconds(dt3));
            Assert.IsFalse(dt3.EqualsIgnoreMillionSeconds(dt4));
        }
    }
}
