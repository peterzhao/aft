
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
    }
}
