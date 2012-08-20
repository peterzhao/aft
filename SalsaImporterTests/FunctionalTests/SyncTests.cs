using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.FunctionalTests
{
    [TestFixture]
    [Category("FunctionalTest")]
    public class SyncTests
    {
        private Supporter _supporterOne;
        private Supporter _supporterTwo;
        private Group _groupOne;
        private Group _groupTwo;

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;

            TestUtils.RemoveAllLocalModelObjects();
            TestUtils.ClearAllSessions();

            TestUtils.RemoveAllSalsa("supporter");
            TestUtils.RemoveAllSalsa("groups");

            _supporterOne = new Supporter
            {
                Email = "newSupporterOne@example.com",
                First_Name = "one",
                Last_Name = "NewSupporter",
                Phone = "416-555-1111"
            };

            _supporterTwo = new Supporter
            {
                Email = "newSupporterTwo@example.com",
                First_Name = "two",
                Last_Name = "NewSupporter",
                Phone = "416-444-1111"
            };

            _groupOne = new Group
            {
                Name = "Group One",
                ReferenceName = "RefGroupOne",
                Description = "Group one description",
            };

            _groupTwo = new Group
            {
                Name = "Group Two",
                ReferenceName = "RefGroupTwo",
                Description = "Group two description",
            };

        }


        [Test]
        public void ShouldPullNewSupportersToLocalDb()
        {
            // Setup...
            TestUtils.CreateSalsa(_supporterOne, _supporterTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalMatches(_supporterOne);
            AssertLocalMatches(_supporterTwo);
        }

        [Test]
        public void ShouldPullToUpdateSupporterInLocalDb()
        {
            // Setup...
            TestUtils.CreateSalsa(_supporterOne, _supporterTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            _supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateSalsa(_supporterOne);

            // Test...
            sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalMatches(_supporterOne);
        }

        [Test]
        public void ShouldPushNewSupportersToSalsa()
        {
            // Setup...
            TestUtils.CreateLocal(_supporterOne, _supporterTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaMatches(_supporterOne);
            AssertSalsaMatches(_supporterTwo);
        }

        [Test]
        public void ShouldPushToUpdateSupporterInSalsa()
        {
            // Setup...
            TestUtils.CreateLocal(_supporterOne, _supporterTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            _supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateLocal(_supporterOne);

            // Test...
            sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaMatches(_supporterOne);
            AssertSalsaMatches(_supporterTwo);
        }
     
      
        [Test]
        public void ShouldPullNewGroupsToLocalDb()
        {
            // Setup...
            TestUtils.CreateSalsa(_groupOne, _groupTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalMatches(_groupOne);
            AssertLocalMatches(_groupTwo);
        }


        [Test]
        public void ShouldPullToUpdateGroupInLocalDb()
        {
            // Setup...
            TestUtils.CreateSalsa(_groupOne, _groupTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            _groupOne.Notes = "Changed the notes for testing";
            TestUtils.UpdateSalsa(_groupOne);

            // Test...
            sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalMatches(_groupOne);
        }

        [Test]
        public void ShouldPushNewGroupToSalsa()
        {
            // Setup...
            TestUtils.CreateLocal(_groupOne, _groupTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaMatches(_groupOne);
            AssertSalsaMatches(_groupTwo);
        }

        [Test]
        public void ShouldPushToUpdateGroupInSalsa()
        {
            // Setup...
            TestUtils.CreateLocal(_groupOne, _groupTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            _groupOne.Notes = "These notes have been updated";
            TestUtils.UpdateLocal(_groupOne);

            // Test...
            sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaMatches(_groupOne);
            AssertSalsaMatches(_groupTwo);
        }
        private void AssertLocalMatches<T>(T salsaObject) where T : class, ISyncObject
        {
            var localRepository = new LocalRepository();
            var localObject = localRepository.GetByExternalKey<T>(salsaObject.Id);

            Assert.AreEqual(salsaObject, localObject);
        }

        private void AssertSalsaMatches<T>(T localObject) where T : class, ISyncObject
        {
            var localRepository = new LocalRepository();
            var localObjectFromDb = localRepository.Get<T>(localObject.Id);

            Debug.Assert(localObjectFromDb.ExternalId != null, string.Format("{0} {1} has no ExternalId in local db", localObject.GetType().Name, localObject.Id));

            var salsaRepository = TestUtils.SalsaRepository;
            var salsaObject = salsaRepository.Get<T>((int)localObjectFromDb.ExternalId);

            Assert.AreEqual(localObject, salsaObject);
        }
       
    }
}