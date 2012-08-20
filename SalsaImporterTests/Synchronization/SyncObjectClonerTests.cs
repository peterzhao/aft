using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncObjectClonerTests
    {
        [Test]
        public void ShouldCloneSupporter()
        {
            var original = new Supporter { Email = "jj@abc.com", First_Name = "Jack", Last_Name = "Joono" };
            var cloned = new SyncObjectCloner().Clone(original);

            Assert.AreEqual(original, cloned);
        }

        [Test]
        public void ShouldCloneGroup()
        {
            var original = new Group { Name = "GroupName", ReferenceName = "ReferenceName", Description = "Description", Notes = "Notes" };
            var cloned = new SyncObjectCloner().Clone(original);


            Assert.AreEqual(original, cloned);
        }
    }
}