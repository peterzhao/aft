using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Aft;

namespace SalsaImporterTests.Aft
{
    [TestFixture]
    public class SupporterTests
    {
        [Test]
        public void ShouldClone()
        {
            var origin = new Supporter {Email = "jj@abc.com", First_Name = "Jack", Last_Name = "Joono", Last_Modified = new DateTime(2012, 3, 4)};
            var cloned = origin.Clone();

            Assert.AreEqual(origin, cloned);
        }
    }
}
