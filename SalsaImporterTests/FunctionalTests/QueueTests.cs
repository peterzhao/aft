using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Salsa;

using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SalsaImporterTests.FunctionalTests
{
    [TestFixture]
    public class QueueTests
    {
        private const string QueueName = "Supporter_SalsaToAftQueue";
        private readonly List<string> _fields = new List<string> { "First_Name", "Last_Name", "Email" };

        private AftDbContext _dbContext;
        
        public QueueTests()
        {
            Config.Environment = Config.Test;
        }


        [SetUp]
        public void SetUp()
        {
            _dbContext = new AftDbContext();

            DynamicSyncObject item;
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
           
            DynamicSyncObject readFromQueue = _dbContext.NextFromQueue(QueueName, _fields);

            _fields.ForEach(f => Assert.AreEqual(supporter.Get(f), readFromQueue.Get(f)));
        }
        
        [Test]
        public void ShouldDeleteFromQueueAndReturnNullWhenEmpty()
        {   
            CreateRandomSupporter();
            DynamicSyncObject readFromQueue = _dbContext.NextFromQueue(QueueName, _fields);
            
            _dbContext.RemoveFromQueue(readFromQueue, QueueName);

            Assert.IsNull(_dbContext.NextFromQueue(QueueName, _fields));
        }

        private DynamicSyncObject CreateRandomSupporter()
        {
            string random = Guid.NewGuid().ToString().Substring(0, 5);
            var supporter = new DynamicSyncObject();
            _fields.ForEach(f => supporter.Add(f, random + f));
            _dbContext.InsertToQueue(supporter, QueueName, _fields);
            return supporter;
        }
    }
}