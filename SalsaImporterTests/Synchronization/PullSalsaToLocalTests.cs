using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;
using SalsaImporterTests.utilities;

namespace SalsaImporterTests.Synchronization
{
    class PullSalsaToLocalTests
    {

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.UnitTest;

            TestUtils.RemoveAllLocal<Supporter>();
            TestUtils.RemoveAllLocal<SyncRun>();

            TestUtils.RemoveAllSalsa("supporter");
        }

        [Test]
        public void ShouldPullNewSubscribersToLocalDb()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSubscribers(out supporterOne, out supporterTwo);

            // Test...
            var pullSalsaToLocal = new PullSalsaToLocal();
            pullSalsaToLocal.run();

            // Verify...
            AssertLocalSubscriberCount(2);
            AssertLocalSupporterMatchesSalsaSupporter(supporterOne);
            AssertLocalSupporterMatchesSalsaSupporter(supporterTwo);
        }


        [Test]
        public void ShouldUpdateSubscriberInLocalDb()
        {
            // Setup...
            Supporter supporterOne;
            Supporter supporterTwo;
            CreateTwoSubscribers(out supporterOne, out supporterTwo);

            var pullSalsaToLocal = new PullSalsaToLocal();
            pullSalsaToLocal.run();

            Thread.Sleep(1000);

            supporterOne.Phone = "416-555-2222";
            TestUtils.UpdateSalsa(supporterOne);


            // Test...
            pullSalsaToLocal.run();

            // Verify...
            AssertLocalSupporterMatchesSalsaSupporter(supporterOne);
        }

        private void AssertLocalSupporterMatchesSalsaSupporter(Supporter salsaSupporter)
        {
            var localRepository = new LocalRepository();
            var localSupporter = localRepository.GetByExternalKey<Supporter>(salsaSupporter.Id);
            Assert.AreEqual(salsaSupporter.Email, localSupporter.Email);
            Assert.AreEqual(salsaSupporter.First_Name, localSupporter.First_Name);
            Assert.AreEqual(salsaSupporter.Last_Name, localSupporter.Last_Name);
            Assert.AreEqual(salsaSupporter.Phone, localSupporter.Phone);
        }

        private static void AssertLocalSubscriberCount(int supporterCount)
        {
            using (var db = new AftDbContext())
            {
                Assert.AreEqual(supporterCount, db.Supporters.Count());
            }
        }



        private static void CreateTwoSubscribers(out Supporter supporterOne, out Supporter supporterTwo)
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

            TestUtils.CreateSalsa(supporterOne, supporterTwo);
        }
    }
}
