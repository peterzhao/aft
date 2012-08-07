using System;
using System.Threading;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class LocalRepositoryTests
    {
        private readonly string MarkerLastName = "LocalRepositoryTests";

        [SetUp]
        public void SetUp()
        {
            TestUtils.RemoveLocal<Supporter>(s => s.Last_Name == MarkerLastName);
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
            int externalId = 100;
            var expectedEmailAddress = Guid.NewGuid().ToString().Substring(0, 6) + "@example.com";

            localRepository.Add(new Supporter
                                    {
                                        ExternalId = externalId, 
                                        Email = expectedEmailAddress, 
                                        Last_Name = MarkerLastName
                                    });

            var retrieved = localRepository.GetByExternalKey<Supporter>(externalId);

            Assert.AreEqual(expectedEmailAddress, retrieved.Email);
        }

        [Test]
        public void ShouldUpdateSupporterAndGetByLocalId()
        {
            var localRepository = new LocalRepository();
            int externalId = 100;
            var expectedCustomStringValue = Guid.NewGuid().ToString();
            var supporter = new Supporter
                                 {
                                     ExternalId = externalId, 
                                     Email = Guid.NewGuid().ToString().Substring(0, 6) + "@example.com",
                                     CustomString0 = "BeforeChange", Last_Name = MarkerLastName
                                 };

            // Setup...
            supporter.Id = localRepository.Add(supporter);

            // Test...
            supporter.CustomString0 = expectedCustomStringValue;
            localRepository.Update(supporter);

            // Verify...
            var retrieved = localRepository.Get<Supporter>(supporter.Id);
            Assert.AreEqual(expectedCustomStringValue, retrieved.CustomString0);
        }

        [Test]
        public void ShouldHaveAnIncrementingCurrentTime()
        {
            var localRepository = new LocalRepository();
            DateTime firstCurrentTime = localRepository.CurrentTime;
            Thread.Sleep(1000);
            DateTime secondCurrentTime = localRepository.CurrentTime;
            Assert.Greater(secondCurrentTime, firstCurrentTime);
        }

    }
}
