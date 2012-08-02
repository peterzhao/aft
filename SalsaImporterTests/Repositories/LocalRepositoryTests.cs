using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporterTests.utilities;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class LocalRepositoryTests
    {
        private readonly string MarkerLastName = "LocalRepositoryTests";

        [SetUp]
        public void SetUp()
        {
            TestUtils.Remove<Supporter>(s => s.Last_Name == MarkerLastName);
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

            localRepository.Add(new Supporter
                                    {
                                        ExternalKey = externalKey, 
                                        Email = expectedEmailAddress, 
                                        Last_Name = MarkerLastName
                                    });

            var retrieved = localRepository.GetByExternalKey<Supporter>(externalKey);

            Assert.AreEqual(expectedEmailAddress, retrieved.Email);
        }

    }
}
