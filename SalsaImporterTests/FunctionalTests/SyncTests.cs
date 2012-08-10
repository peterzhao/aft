﻿using System.Diagnostics;
using System.Linq;
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
        public void ShouldPullNewSubscribersToLocalDb()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSubscribers(out supporterOne, out supporterTwo);
            TestUtils.CreateSalsa(supporterOne, supporterTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertLocalSupporterMatches(supporterOne);
            AssertLocalSupporterMatches(supporterTwo);
        }

        [Test]
        public void ShouldPullToUpdateSubscriberInLocalDb()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSubscribers(out supporterOne, out supporterTwo);
            TestUtils.CreateSalsa(supporterOne, supporterTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateSalsa(supporterOne);

            // Test...
            sync.Run();

            // Verify...
            AssertLocalSupporterMatches(supporterOne);
        }

        [Test]
        public void ShouldPushNewSubscribersToSalsa()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSubscribers(out supporterOne, out supporterTwo);
            TestUtils.CreateLocal(supporterOne, supporterTwo);

            // Test...
            var sync = new Sync();
            sync.Run();

            // Verify...
            AssertSalsaSupporterMatches(supporterOne);
            AssertSalsaSupporterMatches(supporterTwo);
        }

        [Test]
        public void ShouldPushToUpdateSubscriberInSalsa()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSubscribers(out supporterOne, out supporterTwo);
            TestUtils.CreateLocal(supporterOne, supporterTwo);

            var sync = new Sync();
            sync.Run();

            Thread.Sleep(10000);

            supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateLocal(supporterOne);

            // Test...
            sync.Run();

            // Verify...
            AssertSalsaSupporterMatches(supporterOne);
            AssertSalsaSupporterMatches(supporterTwo);
        }

        private static void CreateTwoSubscribers(
            out Supporter supporterOne,
            out Supporter supporterTwo)
        {
            supporterOne = new Supporter
            {
                Email = "newSubscriberOne@example.com",
                First_Name = "one",
                Last_Name = "NewSubscriber",
                Phone = "416-555-1111"
            };
            supporterTwo = new Supporter
            {
                Email = "newSubscriberTwo@example.com",
                First_Name = "two",
                Last_Name = "NewSubscriber",
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

            Debug.Assert(localSupporterFromDb.ExternalId != null, "Supporter " + localSupporter.Id + " has not ExternalId in local db");

            var salsaRepository = TestUtils.SalsaRepository;
            var salsaSupporter = salsaRepository.Get<Supporter>((int)localSupporterFromDb.ExternalId);

            Assert.AreEqual(localSupporter, salsaSupporter);
        }
       
    }
}