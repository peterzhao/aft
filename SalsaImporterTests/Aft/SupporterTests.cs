
using System;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests.Aft
{
    [TestFixture]
    public class SupporterTests
    {

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }

        [Test]
        public void ShouldClone()
        {
            var origin = new Supporter {Email = "jj@abc.com", First_Name = "Jack", Last_Name = "Joono"};
            origin.CustomField["CustomString0"] = "foo";
            var cloned = origin.Clone();

            Assert.AreEqual(origin, cloned);
        }

        [Test]
        public void ShouldCompareSupportersExcludingSomeProperties()
        {
            var supporter1 = new Supporter{Source = "web", Source_Details = "abc", Source_Tracking_Code = "dd", 
                Id = 1234, ExternalId = 5678, ModifiedDate = new DateTime(2012, 12, 25)};
             var supporter2 = new Supporter{Source = "web2", Source_Details = "abc2", Source_Tracking_Code = "dd2", 
                Id = 12342, ExternalId = 56782, ModifiedDate = new DateTime(2012, 12, 26)};

            Assert.AreEqual(supporter1, supporter2);
        }

         [Test]
        public void ShouldCompareSupportersRegardingEmptyAndNullTheSame()
        {
            var supporter1 = new Supporter{State = null};
             var supporter2 = new Supporter{State = ""};

            Assert.AreEqual(supporter1, supporter2);
        }

         [Test]
         public void ShouldCompareSupportersIgnoreMillionSeconds()
         {
             var supporter1 = new Supporter { CustomDateTime0 = new DateTime(2012,7,15,9,30,27,323)};
             var supporter2 = new Supporter { CustomDateTime0 = new DateTime(2012, 7, 15, 9, 30, 27,15) };
             var supporter3 = new Supporter { CustomDateTime0 = new DateTime(2012, 7, 15, 9, 31, 27,15) };

             Assert.AreEqual(supporter1, supporter2);
             Assert.AreNotEqual(supporter1, supporter3);
         }

         [Test]
         public void ShouldCompareSupportersIgnoreDefaultPrivatePotalCode()
         {
             var supporter1 = new Supporter { PRIVATE_Zip_Plus_4 = "0000"};
             var supporter2 = new Supporter { PRIVATE_Zip_Plus_4 = null};
             var supporter4 = new Supporter { PRIVATE_Zip_Plus_4 = ""};
             var supporter5 = new Supporter { PRIVATE_Zip_Plus_4 = "2345"};

             Assert.AreEqual(supporter1, supporter2);
             Assert.AreEqual(supporter1, supporter4);
             Assert.AreNotEqual(supporter1, supporter5);
         }

        [Test]
        public void ShouldCompaireCustomFieldValues()
        {
            var supporter1 = new Supporter {Email = "foo@abc.com"};
            var supporter2 = new Supporter {Email = "foo@abc.com"};

            supporter1.CustomField["CustomString0"] = "doo";
            supporter2.CustomField["CustomString0"] = "foo";
            Assert.AreNotEqual(supporter1, supporter2);
            
            supporter1.CustomField["CustomString0"] = "doo";
            supporter2.CustomField["CustomString0"] = "doo";
            Assert.AreEqual(supporter1, supporter2);
        }

      
    }
}
