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
                                    supporter_KEY = 123456,
                                    Last_Modified = new DateTime(2012, 3, 15),
                                    Date_Created = new DateTime(2012, 3, 16),
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
                                    CustomBoolean1 = false
                                };
            var nameValues = new SupporterMapper().ToNameValues(supporter);

            Assert.AreEqual(nameValues["supporter_KEY"], "123456");
            Assert.IsNull(nameValues["Id"]);
            Assert.IsNull(nameValues["Last_Modified"]);
            Assert.IsNull(nameValues["Date_Created"]);
            Assert.AreEqual(nameValues["Title"], "myTitle");
            Assert.AreEqual(nameValues["First_Name"], "Tom");
            Assert.AreEqual(nameValues["MI"], "MI");
            Assert.AreEqual(nameValues["Last_Name"], "Alex");
            Assert.AreEqual(nameValues["Suffix"], "Mr.");
            Assert.AreEqual(nameValues["Email"], "ATom@abc.com");
            //Assert.AreEqual(nameValues["Receive_Email"], "-9");
            //Assert.AreEqual(nameValues["Email_Preference"], "HTML");
            Assert.IsNull(nameValues["Last_Bounce"]);
            //Assert.AreEqual(nameValues["Receive_Phone_Blasts"], "8");
            Assert.AreEqual(nameValues["Phone"], "14168805783");
            Assert.AreEqual(nameValues["Cell_Phone"], "16473451234");
            Assert.AreEqual(nameValues["Phone_Provider"], "Bell");
            Assert.AreEqual(nameValues["Work_Phone"], "16472911234");
            Assert.AreEqual(nameValues["Pager"], "14163456789");
            Assert.AreEqual(nameValues["Home_Fax"], "14168906789");
            Assert.AreEqual(nameValues["Work_Fax"], "14168903456");
            Assert.AreEqual(nameValues["Street"], "100 Main Street");
            Assert.AreEqual(nameValues["Street_2"], "Apt 100");
            Assert.AreEqual(nameValues["Street_3"], "Suit 201");
            Assert.AreEqual(nameValues["City"], "Toronto");
            Assert.AreEqual(nameValues["State"], "ON");
            Assert.AreEqual(nameValues["Zip"], "M2B 2C9");
            Assert.AreEqual(nameValues["PRIVATE_Zip_Plus_4"], "");
            Assert.AreEqual(nameValues["County"], "Woods");
            Assert.AreEqual(nameValues["District"], "NA");
            Assert.AreEqual(nameValues["Country"], "Canada");
            Assert.AreEqual(nameValues["Latitude"], "57.34");
            Assert.AreEqual(nameValues["Longitude"], "104.24");
            Assert.AreEqual(nameValues["Organization"], "AFT");
            Assert.AreEqual(nameValues["Department"], "Accounting");
            Assert.AreEqual(nameValues["Occupation"], "Teacher");
            Assert.AreEqual(nameValues["Instant_Messenger_Service"], "YAHOO");
            Assert.AreEqual(nameValues["Instant_Messenger_Name"], "atom@yahoo.com");
            Assert.AreEqual(nameValues["Web_Page"], "http://www.atom.com");
            Assert.AreEqual(nameValues["Alternative_Email"], "atom@gmail.com");
            Assert.AreEqual(nameValues["Other_Data_1"], "data1");
            Assert.AreEqual(nameValues["Other_Data_2"], "data2");
            Assert.AreEqual(nameValues["Other_Data_3"], "data3");
            Assert.AreEqual(nameValues["Notes"], "my notes");
            Assert.AreEqual(nameValues["Source"], "AFTTesting");
            Assert.AreEqual(nameValues["Source_Details"], "AFT NA");
            Assert.AreEqual(nameValues["Source_Tracking_Code"], "03");
            Assert.AreEqual(nameValues["Tracking_Code"], "001");
            Assert.AreEqual(nameValues["Status"], "Active");
            Assert.AreEqual(nameValues["uid"], "456");
            Assert.AreEqual(nameValues["Timezone"], "");
            Assert.AreEqual(nameValues["Language_Code"], "en");
            Assert.AreEqual(nameValues["CustomBoolean0"], "True");
            Assert.AreEqual(nameValues["CustomBoolean1"], "False");
            
        }

        [Test]
        public void ShouldMapToNameValuesWithoutNullValue()
        {
            var supporter = new Supporter {Id = 3456, Email = "jim@abc.com"};
            var nameValues = new SupporterMapper().ToNameValues(supporter);
            Assert.AreEqual(nameValues["Email"], "jim@abc.com");
            Assert.AreEqual(2, nameValues.Keys.Count);
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
      <CustomDateTime0>Wed Jul 25 2012 09:17:50 GMT-0400 (EDT)</CustomDateTime0>
      <key>32294089</key>
      <object>supporter</object>
</item>";
            XElement element = XDocument.Parse(xml).Root;
            Supporter supporter = (Supporter)new SupporterMapper().ToObject(element);

            Assert.AreEqual("peter", supporter.First_Name);
            Assert.AreEqual("zhao", supporter.Last_Name);
            Assert.AreEqual("peter@abc.com", supporter.Email);
            Assert.AreEqual(32294089, supporter.supporter_KEY);

            Assert.AreEqual(new DateTimeOffset(new DateTime(2012, 7, 25, 9, 17, 50), new TimeSpan(-4, 0, 0)).UtcDateTime, 
                supporter.CustomDateTime0);
            Assert.AreEqual(-45.234f, supporter.Latitude);

        }

    }
}