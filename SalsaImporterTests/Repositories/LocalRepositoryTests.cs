using System;
using System.Linq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class LocalRepositoryTests
    {
        private readonly string MarkerLastName = "LocalRepositoryTests";

        [SetUp]
        public void SetUp()
        {
            RemoveTestSupporters();
        }

        private void RemoveTestSupporters()
        {
            using (var db = new AftDbContext())
            {
                var supporters = db.Supporters.Where(s => s.Last_Name == MarkerLastName);
                supporters.ToList().ForEach(s => db.Supporters.Remove(s));
                db.SaveChanges();
            }
        }

        [Test]
        public void ShouldReturnNullWhenExternalIdNotFound()
        {
            var localRepository = new LocalRepository();

            var retrieved = localRepository.GetByExternalKey<Supporter>(100);

            Assert.Null(retrieved);
        }

        [Test]
        public void ShouldAddSupporterAndRetrieveSupporterByExternalId()
        {
            var localRepository = new LocalRepository();

            int externalKey = 100;
            var expectedEmailAddress = Guid.NewGuid().ToString().Substring(0, 6) + "@example.com";

            localRepository.Add(new Supporter { ExternalKey = externalKey, Email = expectedEmailAddress, Last_Name = MarkerLastName});

            var retrieved = localRepository.GetByExternalKey<Supporter>(externalKey);

            Assert.AreEqual(expectedEmailAddress, retrieved.Email);
        
        }

        [TearDown]
        public void TearDown()
        {
            RemoveTestSupporters();
        }

    }
}
