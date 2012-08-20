
using System;
using NUnit.Framework;
using SalsaImporter.Aft;

namespace SalsaImporterTests.Aft
{
    [TestFixture]
    public class GroupTests
    {

        [Test]
        public void ShouldCompareOnFields()
        {
            var group = new Group { Name = "GroupName", ReferenceName = "ReferenceName", Description = "Description", Notes = "Notes" };
            
            // Identity
            Assert.AreEqual(group, group);

            Assert.AreEqual(group, new Group { Name = "GroupName", ReferenceName = "ReferenceName", Description = "Description", Notes = "Notes" });
            Assert.AreNotEqual(group, new Group { Name = "GroupNameX", ReferenceName = "ReferenceName", Description = "Description", Notes = "Notes" });
            Assert.AreNotEqual(group, new Group { Name = "GroupName", ReferenceName = "ReferenceNameX", Description = "DescriptionX", Notes = "Notes" });
            Assert.AreNotEqual(group, new Group { Name = "GroupName", ReferenceName = "ReferenceName", Description = "Description", Notes = "NotesX" });

           

        }

        [Test]
        public void ShouldNotCompareOnIdExternalOrModifiedDate()
        {
            var group = new Group { Name = "GroupName", Id = 1, ExternalId = 1, ModifiedDate = new DateTime(2012, 1, 1) };

            Assert.AreEqual(group, new Group { Name = "GroupName", Id = 2, ExternalId = 1, ModifiedDate = new DateTime(2012, 1, 1) });
            Assert.AreEqual(group,   new Group { Name = "GroupName", Id = 1, ExternalId = 2, ModifiedDate = new DateTime(2012, 1, 1) });
            Assert.AreEqual(group,   new Group { Name = "GroupName", Id = 1, ExternalId = 1, ModifiedDate = new DateTime(2012, 2, 1) });
        }

       
    }
}
