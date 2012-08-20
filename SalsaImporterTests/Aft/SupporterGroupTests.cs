
using System;
using NUnit.Framework;
using SalsaImporter.Aft;

namespace SalsaImporterTests.Aft
{
    [TestFixture]
    public class SupporterGroupTests
    {
        [Test]
        public void ShouldCompareOnFields()
        {
            var supporterGroup = new SupporterGroup { SupporterId = 123, GroupId = 456 };
            
            // Identity
            Assert.AreEqual(supporterGroup, supporterGroup);

            Assert.AreEqual(supporterGroup, new SupporterGroup { SupporterId = 123, GroupId = 456 });
            Assert.AreNotEqual(supporterGroup, new SupporterGroup { SupporterId = 223, GroupId = 456 });
            Assert.AreNotEqual(supporterGroup, new SupporterGroup { SupporterId = 123, GroupId = 256 });
        }

        [Test]
        public void ShouldNotCompareOnIdExternalOrModifiedDate()
        {
            var group = new SupporterGroup { SupporterId = 123, GroupId = 456, Id = 1, ExternalId = 1, ModifiedDate = new DateTime(2012, 1, 1) };

            Assert.AreEqual(group, new SupporterGroup { SupporterId = 123, GroupId = 456, Id = 2, ExternalId = 1, ModifiedDate = new DateTime(2012, 1, 1) });
            Assert.AreEqual(group, new SupporterGroup { SupporterId = 123, GroupId = 456, Id = 1, ExternalId = 2, ModifiedDate = new DateTime(2012, 1, 1) });
            Assert.AreEqual(group, new SupporterGroup { SupporterId = 123, GroupId = 456, Id = 1, ExternalId = 1, ModifiedDate = new DateTime(2012, 2, 1) });
        }   
    }
}
