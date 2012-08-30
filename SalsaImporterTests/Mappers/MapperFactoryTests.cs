using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class MapperFactoryTests
    {
        [SetUp]
        public void Setup()
        {
            Config.Environment = Config.Test;
        }
        [Test]
        public void ShouldGetMapperForSupporter()
        {
            var mapper  = new MapperFactory().GetMapper("supporter");
            Assert.IsTrue(mapper.Mappings.Any(m => m.SalsaField == "Email"));
            Assert.IsTrue(mapper.Mappings.Any(m => m.SalsaField == "First_Name"));
            Assert.IsTrue(mapper.Mappings.Any(m => m.SalsaField == "Last_Name"));
        }
    }
}
