using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Repositories
{
    [TestFixture]
    public class SalsaRepositoryTests
    {
        private SalsaRepository repository;

        [SetUp]
        public void SetUp()
        {
            repository = new SalsaRepository();
        }
        [Test]
        public void ShouldGetObjectBeExternalKey()
        {
            //var externalKey = 1234;
            //ISyncObject obj = repository.GetByExternalKey(externalKey);
        }
    }
}
