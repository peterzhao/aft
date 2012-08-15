using System;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    public class SupporterMapperTests
    {
      
        [Test]
        public void ShouldMapToNameValues()
        {
            var supporter = new Supporter
                                {
                                    Id = 456,
                                    ExternalId = 123456,
                                    Title = "myTitle",
                                    First_Name = "Tom",
                                    MI = "MI",
                                    Last_Name = "Alex",
                                    Suffix = "Mr.",
                                    Email = "ATom@abc.com",
                                    //Receive_Email = -9,
                                    Email_Preference = "HTML",
                                    Last_Bounce = new DateTime(2012, 5, 13),
                                    //Receive_Phone_Blasts = 8,
                                    Phone = "14168805783",
                                    Cell_Phone = "16473451234",
                                    Phone_Provider = "Bell",
                                    Work_Phone = "16472911234",
                                    Pager = "14163456789",
                                    Home_Fax = "14168906789",
                                    Work_Fax = "14168903456",
                                    Street = "100 Main Street",
                                    Street_2 = "Apt 100",
                                    Street_3 = "Suit 201",
                                    City = "Toronto",
                                    State = "ON",
                                    Zip = "M2B 2C9",
                                    PRIVATE_Zip_Plus_4 = "",
                                    County = "Woods",
                                    District = "NA",
                                    Country = "Canada",
                                    Latitude = 57.34f,
                                    Longitude = 104.24f,
                                    Organization = "AFT",
                                    Department = "Accounting",
                                    Occupation = "Teacher",
                                    Instant_Messenger_Service = "YAHOO",
                                    Instant_Messenger_Name = "atom@yahoo.com",
                                    Web_Page = "http://www.atom.com",
                                    Alternative_Email = "atom@gmail.com",
                                    Other_Data_1 = "data1",
                                    Other_Data_2 = "data2",
                                    Other_Data_3 = "data3",
                                    Notes = "my notes",
                                    Source = "AFTTesting",
                                    Source_Details = "AFT NA",
                                    Source_Tracking_Code = "03",
                                    Tracking_Code = "001",
                                    Status = "Active",
                                    Timezone = "",
                                    Language_Code = "en",
                                    CustomBoolean0 = true,
                                    CustomBoolean1 = false,
                                    CustomDateTime0 = new DateTime(2012,7,15, 23,12,2),
                                    Chapter_Key = 57,
                                };
            var nameValues = new SupporterMapper().ToNameValues(supporter);

            Assert.AreEqual("2012-07-15 23:12:02", nameValues["CustomDateTime0"]);
            Assert.AreEqual("456", nameValues["supporter_KEY"]);
            Assert.AreEqual("myTitle", nameValues["Title"]);
            Assert.AreEqual("Tom", nameValues["First_Name"]);
            Assert.AreEqual("MI", nameValues["MI"]);
            Assert.AreEqual("Alex", nameValues["Last_Name"]);
            Assert.AreEqual("Mr.", nameValues["Suffix"]);
            Assert.AreEqual("ATom@abc.com", nameValues["Email"]);
            //Assert.AreEqual(nameValues["Receive_Email"], "-9");
            //Assert.AreEqual(nameValues["Email_Preference"], "HTML");
            Assert.AreEqual("2012-05-13 00:00:00", nameValues["Last_Bounce"]);
            //Assert.AreEqual(nameValues["Receive_Phone_Blasts"], "8");
            Assert.AreEqual("14168805783", nameValues["Phone"]);
            Assert.AreEqual("16473451234", nameValues["Cell_Phone"]);
            Assert.AreEqual("Bell", nameValues["Phone_Provider"]);
            Assert.AreEqual("16472911234", nameValues["Work_Phone"]);
            Assert.AreEqual("14163456789", nameValues["Pager"]);
            Assert.AreEqual("14168906789", nameValues["Home_Fax"]);
            Assert.AreEqual("14168903456", nameValues["Work_Fax"]);
            Assert.AreEqual("100 Main Street", nameValues["Street"]);
            Assert.AreEqual("Apt 100", nameValues["Street_2"]);
            Assert.AreEqual("Suit 201", nameValues["Street_3"]);
            Assert.AreEqual("Toronto", nameValues["City"]);
            Assert.AreEqual("ON", nameValues["State"]);
            Assert.AreEqual("M2B 2C9", nameValues["Zip"]);
            Assert.AreEqual("", nameValues["PRIVATE_Zip_Plus_4"]);
            Assert.AreEqual("Woods", nameValues["County"]);
            Assert.AreEqual("NA", nameValues["District"]);
            Assert.AreEqual("Canada", nameValues["Country"]);
            Assert.AreEqual("57.34", nameValues["Latitude"]);
            Assert.AreEqual("104.24", nameValues["Longitude"]);
            Assert.AreEqual("AFT", nameValues["Organization"]);
            Assert.AreEqual("Accounting", nameValues["Department"]);
            Assert.AreEqual("Teacher", nameValues["Occupation"]);
            Assert.AreEqual("YAHOO", nameValues["Instant_Messenger_Service"]);
            Assert.AreEqual("atom@yahoo.com", nameValues["Instant_Messenger_Name"]);
            Assert.AreEqual("http://www.atom.com", nameValues["Web_Page"]);
            Assert.AreEqual("atom@gmail.com", nameValues["Alternative_Email"]);
            Assert.AreEqual("data1", nameValues["Other_Data_1"]);
            Assert.AreEqual("data2", nameValues["Other_Data_2"]);
            Assert.AreEqual("data3", nameValues["Other_Data_3"]);
            Assert.AreEqual("my notes", nameValues["Notes"]);
            Assert.AreEqual("AFTTesting", nameValues["Source"]);
            Assert.AreEqual("AFT NA", nameValues["Source_Details"]);
            Assert.AreEqual("03", nameValues["Source_Tracking_Code"]);
            Assert.AreEqual("001", nameValues["Tracking_Code"]);
            Assert.AreEqual("Active", nameValues["Status"]);
            Assert.AreEqual("123456", nameValues["uid"]);
            Assert.AreEqual("", nameValues["Timezone"]);
            Assert.AreEqual("en", nameValues["Language_Code"]);
            Assert.AreEqual("1", nameValues["CustomBoolean0"]);
            Assert.AreEqual("0", nameValues["CustomBoolean1"]);
            Assert.AreEqual("57", nameValues["chapter_KEY"]);
            
        }

        [Test]
        public void ShouldMapToNameValuesWithoutNullValue()
        {
            var supporter = new Supporter {Id = 3456, Email = "jim@abc.com"};
            var nameValues = new SupporterMapper().ToNameValues(supporter);
            Assert.AreEqual("jim@abc.com", nameValues["Email"]);
            Assert.IsNull(nameValues["chapter_KEY"]);
            Assert.AreEqual(12, nameValues.Keys.Count);
        }

        [Test] 
        public void ShouldMapToSupporter()
        {
            var xml = @"<item>
      <supporter_KEY>32294089</supporter_KEY>
      <Email>peter@abc.com</Email>
      <First_Name>peter</First_Name>
      <Last_Name>zhao</Last_Name>
      <Latitude>-45.234</Latitude>
      <CustomDateTime0>Wed Jul 25 2012 09:17:50 GMT-0500 (EDT)</CustomDateTime0>
      <key>32294089</key>
      <Other_Data_1/>
      <Other_Data_2></Other_Data_2>
      <object>supporter</object>
      <Longitude/>
      <CustomBoolean0>false</CustomBoolean0>
      <CustomBoolean1>true</CustomBoolean1>
      <chapter_KEY>57</chapter_KEY>
</item>";
            XElement element = XDocument.Parse(xml).Root;
            Supporter supporter = (Supporter)new SupporterMapper().ToObject(element);

            Assert.AreEqual("peter", supporter.First_Name);
            Assert.AreEqual("zhao", supporter.Last_Name);
            Assert.AreEqual("peter@abc.com", supporter.Email);
            Assert.AreEqual(32294089, supporter.Id);
            Assert.AreEqual(57, supporter.Chapter_Key);
            Assert.IsFalse(supporter.CustomBoolean0);
            Assert.IsTrue(supporter.CustomBoolean1);
            Assert.IsNull(supporter.Other_Data_1);
            Assert.IsNull(supporter.Other_Data_2);
            Assert.IsNull(supporter.Longitude);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2012, 7, 25, 9, 17, 50), new TimeSpan(-5, 0, 0)).LocalDateTime, 
                supporter.CustomDateTime0);
            Assert.AreEqual(-45.234f, supporter.Latitude);

        }

    }
}