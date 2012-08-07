using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class LocalRepositoryTests
    {
        
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.UnitTest;
            TestUtils.RemoveAllSupporter();
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
                                        Last_Name = "LastName"
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
            var supporter = GenerateSupporter(externalId);

            // Setup...
            supporter.Id = localRepository.Add(supporter);

            // Test...
            supporter.CustomString0 = expectedCustomStringValue;
            localRepository.Update(supporter);

            // Verify...
            var retrieved = localRepository.Get<Supporter>(supporter.Id);
            Assert.AreEqual(expectedCustomStringValue, retrieved.CustomString0);
        }

        private static Supporter GenerateSupporter(int externalId)
        {
            var supporter = GenerateSupporter("FirstName");
            supporter.ExternalId = externalId;
            return supporter;
        }

        private static Supporter GenerateSupporter(string firstName)
        {
            return new Supporter
                                {
                                    Email = Guid.NewGuid().ToString().Substring(0, 6) + "@example.com",
                                    CustomString0 = "BeforeChange", 
                                    First_Name =  firstName,
                                    Last_Name = "LastName"
                                };
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

        [Test]
        public void ShouldGetBatchOfObjectsFromLocal()
        {
            var supporterOne = GenerateSupporter("One");
            var supporterTwo = GenerateSupporter("Two");
            var supporterThree = GenerateSupporter("Three");
            var supporterFour = GenerateSupporter("Four");
            var supporterFive = GenerateSupporter("Five");
            TestUtils.CreateLocal(supporterOne, supporterTwo, supporterThree, supporterFour, supporterFive);

            var localRepository = new LocalRepository();
            var batch1 = localRepository.GetBatchOfObjects<Supporter>(2, 0, new DateTime(1991, 1, 1)).ToList();
            var batch2 = localRepository.GetBatchOfObjects<Supporter>(2, supporterTwo.Id, new DateTime(1991, 1, 1)).ToList();
            var batch3 = localRepository.GetBatchOfObjects<Supporter>(2, supporterFour.Id, new DateTime(1991, 1, 1)).ToList();

            Assert.AreEqual(2, batch1.Count);
            Assert.AreEqual(2, batch2.Count);
            Assert.AreEqual(1, batch3.Count);

            Assert.AreEqual(supporterOne, batch1.First());
            Assert.AreEqual(supporterTwo, batch1.Last());
            Assert.AreEqual(supporterThree, batch2.First());
            Assert.AreEqual(supporterFour, batch2.Last());
            Assert.AreEqual(supporterFive, batch3.First());
        }

    }
}
