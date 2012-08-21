using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;
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
            Config.Environment = Config.Test;
            TestUtils.RemoveAllLocalModelObjects();
        }

        [Test]
        public void ShouldReturnNullWhenExternalIdNotFound()
        {
            var localRepository = new LocalRepository();

            var retrieved = localRepository.GetByExternalKey<Supporter>(100);

            Assert.Null(retrieved);
        }

        [Test]
        public void ShouldAddAndGetSupporter()
        {
            var repository = new LocalRepository();
            var name = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter = new Supporter {Email = name + "@abc.com", First_Name = name, Last_Name = "testing"};
            var key = repository.Add(supporter);
            var retrived = repository.Get<Supporter>(key);

            Assert.IsTrue(supporter.EqualValues(retrived));
        }

        [Test]
        public void ShouldAddSupporterAndRetrieveSupporterByExternalId()
        {
            var localRepository = new LocalRepository();
            SyncEventArgs syncEventArgs = null;
            localRepository.NotifySyncEvent += (sender, args) => syncEventArgs = args;
            int externalId = 100;
            var expectedEmailAddress = Guid.NewGuid().ToString().Substring(0, 6) + "@example.com";

            var supporter = new Supporter
            {
                ExternalId = externalId,
                Email = expectedEmailAddress,
                Last_Name = "LastName"
            };
            localRepository.Add(supporter);

            var retrieved = localRepository.GetByExternalKey<Supporter>(externalId);

            Assert.AreEqual(expectedEmailAddress, retrieved.Email);

            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(localRepository, syncEventArgs.Destination);
            Assert.AreEqual(supporter, syncEventArgs.SyncObject);
            Assert.AreEqual(SyncEventType.Add, syncEventArgs.EventType);
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
            SyncEventArgs syncEventArgs = null;
            localRepository.NotifySyncEvent += (sender, args) => syncEventArgs = args;
            // Test...
            supporter.CustomString0 = expectedCustomStringValue;
          
            localRepository.Update(supporter);

            // Verify...
            var retrieved = localRepository.Get<Supporter>(supporter.Id);
            Assert.AreEqual(expectedCustomStringValue, retrieved.CustomString0);
            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(localRepository, syncEventArgs.Destination);
            Assert.AreEqual(supporter, syncEventArgs.SyncObject);
            Assert.AreEqual(SyncEventType.Update, syncEventArgs.EventType);
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

            Assert.IsTrue(supporterOne.EqualValues(batch1.First()));
            Assert.IsTrue(supporterTwo.EqualValues(batch1.Last()));
            Assert.IsTrue(supporterThree.EqualValues(batch2.First()));
            Assert.IsTrue(supporterFour.EqualValues(batch2.Last()));
            Assert.IsTrue(supporterFive.EqualValues(batch3.First()));
        }

        [Test]
        public void ShouldAddGroupAndRetrieveByExternalId()
        {
            var localRepository = new LocalRepository();
            int externalId = 100;

            var group = new Group
            {
                ExternalId = externalId,
                Name = "GroupName",
                ReferenceName = "GroupReferenceName",
                Description = "Description...",
                Notes = "Notes..."
            };
            localRepository.Add(group);

            var retrievedGroup = localRepository.GetByExternalKey<Group>(externalId);

            Assert.AreEqual(group, retrievedGroup);
        }
    }

    

}
