using System;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests
{
    [TestFixture]
    public class AftDbContextTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
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
