
using System.Linq;
using NUnit.Framework;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncObjectTests
    {
        [Test]
        public void ShouldSetAndGetValues()
        {
            const string fieldName = "first_name";
            const string expectedValue = "tom";
            const string expectedChangedValue = "peter";

            var obj = new SyncObject("supporter");

            obj[fieldName] = expectedValue;
            Assert.AreEqual(expectedValue, obj[fieldName]);

            obj[fieldName] = expectedChangedValue;
            Assert.AreEqual(expectedChangedValue, obj[fieldName]);
        }

        [Test]
        public void ShouldGetFieldNames()
        {
            var obj = new SyncObject("supporter");
            obj["FirstName"] = "peter";
            obj["LastName"] = "Zhao";
            Assert.IsTrue(obj.FieldNames.Any(f => f == "FirstName"));
            Assert.IsTrue(obj.FieldNames.Any(f => f == "LastName"));

        }



    }
}
