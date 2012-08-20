using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.FunctionalTests
{
    [TestFixture]
    [Category("FunctionalTest")]
    public class SyncTests
    {

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;

            TestUtils.RemoveAllLocal<Supporter>();
            TestUtils.ClearAllSessions();

            TestUtils.RemoveAllSalsa("supporter");
        }


        [Test]
        public void ShouldPullNewSupportersToLocalDb()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSupporters(out supporterOne, out supporterTwo);
            TestUtils.CreateSalsa(supporterOne, supporterTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalSupporterMatches(supporterOne);
            AssertLocalSupporterMatches(supporterTwo);
        }

        [Test]
        public void ShouldPullToUpdateSupporterInLocalDb()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSupporters(out supporterOne, out supporterTwo);
            TestUtils.CreateSalsa(supporterOne, supporterTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateSalsa(supporterOne);

            // Test...
            sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalSupporterMatches(supporterOne);
        }

        [Test]
        public void ShouldPushNewSupportersToSalsa()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSupporters(out supporterOne, out supporterTwo);
            TestUtils.CreateLocal(supporterOne, supporterTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaSupporterMatches(supporterOne);
            AssertSalsaSupporterMatches(supporterTwo);
        }

        [Test]
        public void ShouldPushToUpdateSupporterInSalsa()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSupporters(out supporterOne, out supporterTwo);
            TestUtils.CreateLocal(supporterOne, supporterTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateLocal(supporterOne);

            // Test...
            sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaSupporterMatches(supporterOne);
            AssertSalsaSupporterMatches(supporterTwo);
        }

        private static void CreateTwoSupporters(
            out Supporter supporterOne,
            out Supporter supporterTwo)
        {
            supporterOne = new Supporter
            {
                Email = "newSupporterOne@example.com",
                First_Name = "one",
                Last_Name = "NewSupporter",
                Phone = "416-555-1111"
            };
            supporterTwo = new Supporter
            {
                Email = "newSupporterTwo@example.com",
                First_Name = "two",
                Last_Name = "NewSupporter",
                Phone = "416-444-1111"
            };
        }

        private void AssertLocalSupporterMatches(Supporter salsaSupporter)
        {
            var localRepository = new LocalRepository();
            var localSupporter = localRepository.GetByExternalKey<Supporter>(salsaSupporter.Id);

            Assert.AreEqual(salsaSupporter, localSupporter);
        }

        private void AssertSalsaSupporterMatches(Supporter localSupporter)
        {
            var localRepository = new LocalRepository();
            var localSupporterFromDb = localRepository.Get<Supporter>(localSupporter.Id);

            Debug.Assert(localSupporterFromDb.ExternalId != null, "Supporter " + localSupporter.Id + " has no ExternalId in local db");

            var salsaRepository = TestUtils.SalsaRepository;
            var salsaSupporter = salsaRepository.Get<Supporter>((int)localSupporterFromDb.ExternalId);

            Assert.AreEqual(localSupporter, salsaSupporter);
        }
       
    }
}