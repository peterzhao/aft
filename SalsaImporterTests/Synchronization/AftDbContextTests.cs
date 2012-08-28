using System;
using System.Collections.Generic;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class AftDbContextTests
    {
        private const string QueueName = "Supporter_SalsaToAftQueue";
        private readonly List<string> _fields = new List<string> { "First_Name", "Last_Name", "Email" };

        private AftDbContext _dbContext;
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            _dbContext = new AftDbContext();

            SyncObject item;
            while (null != (item = _dbContext.NextFromQueue(QueueName, _fields)))
            {
                _dbContext.RemoveFromQueue(item, QueueName);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public void ShouldInsertToQueueAndPullFromQueue()
        {
            var supporter = CreateRandomSupporter();

            SyncObject readFromQueue = _dbContext.NextFromQueue(QueueName, _fields);

            _fields.ForEach(f => Assert.AreEqual(supporter[f], readFromQueue[f]));
        }

        [Test]
        public void ShouldDeleteFromQueueAndReturnNullWhenEmpty()
        {
            CreateRandomSupporter();
            SyncObject readFromQueue = _dbContext.NextFromQueue(QueueName, _fields);

            _dbContext.RemoveFromQueue(readFromQueue, QueueName);

            Assert.IsNull(_dbContext.NextFromQueue(QueueName, _fields));
        }

        private SyncObject CreateRandomSupporter()
        {
            string random = Guid.NewGuid().ToString().Substring(0, 5);
            var supporter = new SyncObject("supporter");
            _fields.ForEach(f => supporter[f] =  random + f);
            _dbContext.InsertToQueue(supporter, QueueName, _fields);
            return supporter;
        }

        public void Db(Action<AftDbContext> action)
        {
            using(var db = new AftDbContext())
            {
                action(db);
            }
        }
    }
}
