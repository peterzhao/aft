using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    public class GroupMapperTests
    {
        [Test]
        public void ShouldMapToNameValues()
        {
            var group = new Group
            {
                Id = 123,
                ExternalId = 123456,
                Name = "Group Name",
                ReferenceName = "Reference Name",
                Description = "This is the description",
                Notes = "These are the notes"
            };
            var nameValues = new GroupMapper().ToNameValues(group);

            Assert.AreEqual("123", nameValues["key"]);
            Assert.AreEqual("123456", nameValues["external_ID"]);
            Assert.AreEqual("Group Name", nameValues["Group_Name"]);
            Assert.AreEqual("Reference Name", nameValues["Reference_Name"]);
            Assert.AreEqual("This is the description", nameValues["Description"]);
            Assert.AreEqual("These are the notes", nameValues["Notes"]);
        }

        [Test]
        public void ShouldAddKeyWhenUpdating()
        {
            string magicSalsaName = "key";
            var group = new Group
            {
                Id = 123,
                ExternalId = 123456,
                Name = "Group Name",
                ReferenceName = "Reference Name",
                Description = "This is the description",
                Notes = "These are the notes"
            };
            var target = new GroupMapper();
            var collection = target.ToNameValues(group);

            Assert.IsFalse(string.IsNullOrEmpty(collection[magicSalsaName]));
        }

        [Test]
        public void ShouldMapFromSalsa()
        {
            var inputXml =  @" <item>
      <groups_KEY>5402</groups_KEY>
      <organization_KEY>4013</organization_KEY>
      <chapter_KEY>533</chapter_KEY>
      <Last_Modified>Tue Jun 22 2010 07:35:18 GMT-0400 (EDT)</Last_Modified>
      <Date_Created>Tue Jun 22 2010 07:35:18 GMT-0400 (EDT)</Date_Created>
      <Group_Name>AFT Washington: Legisletter</Group_Name>
      <Reference_Name>Reference Name</Reference_Name>
      <parent_KEY/>
      <Description>This is a description</Description>
      <Notes>These are notes...</Notes>
      <Display_To_User_BOOLVALUE>false</Display_To_User_BOOLVALUE>
      <Display_To_User>false</Display_To_User>
      <Listserve_Type/>
      <Subscription_Type/>
      <Manager/>
      <Moderator_Emails/>
      <Subject_Prefix/>
      <Listserve_Responses/>
      <Append_Header/>
      <Append_Footer/>
      <Custom_Headers/>
      <Listserve_Options/>
      <external_ID>12345</external_ID>
      <From_Email/>
      <From_Name/>
      <Reply_To/>
      <Headers_To_Remove/>
      <Confirmation_Message/>
      <Auto_Update_BOOLVALUE>false</Auto_Update_BOOLVALUE>
      <Auto_Update>false</Auto_Update>
      <query_KEY/>
      <Smart_Group_Options/>
      <Smart_Group_Error/>
      <enable_statistics_BOOLVALUE>false</enable_statistics_BOOLVALUE>
      <enable_statistics>false</enable_statistics>
      <join_email_trigger_KEYS/>
      <key>5402</key>
      <object>groups</object>
    </item>";

            XElement element = XDocument.Parse(inputXml).Root;
            Group group = (Group)new GroupMapper().ToObject(element);

            Assert.AreEqual(5402, group.Id);
            Assert.AreEqual(12345, group.ExternalId);
            Assert.AreEqual("AFT Washington: Legisletter", group.Name);
            Assert.AreEqual("Reference Name", group.ReferenceName);
            Assert.AreEqual("This is a description", group.Description);
            Assert.AreEqual("These are notes...", group.Notes);

        }
    }
}
