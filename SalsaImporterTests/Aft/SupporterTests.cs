
using System;
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
            var origin = new Supporter {Email = "jj@abc.com", First_Name = "Jack", Last_Name = "Joono"};
            var cloned = origin.Clone();

            Assert.AreEqual(origin, cloned);
        }

        [Test]
        public void ShouldCompaireSupportersExcludingSomeProperties()
        {
            var supporter1 = new Supporter{Source = "web", Source_Details = "abc", Source_Tracking_Code = "dd", 
                Id = 1234, ExternalId = 5678, ModifiedDate = new DateTime(2012, 12, 25)};
             var supporter2 = new Supporter{Source = "web2", Source_Details = "abc2", Source_Tracking_Code = "dd2", 
                Id = 12342, ExternalId = 56782, ModifiedDate = new DateTime(2012, 12, 26)};

            Assert.AreEqual(supporter1, supporter2);
        }

         [Test]
        public void ShouldCompaireSupportersRegardingEmptyAndNullTheSame()
        {
            var supporter1 = new Supporter{State = null};
             var supporter2 = new Supporter{State = ""};

            Assert.AreEqual(supporter1, supporter2);
        }
    }
}
