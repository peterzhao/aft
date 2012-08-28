using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    public class MapperFactoryTests
    {
        [Test]
        public void ShouldGetMapperForSupporter()
        {
            var mapper  = new MapperFactory().GetMapper("Supporter");
            Assert.AreEqual(typeof(SupporterMapper), mapper.GetType());
        }
    }
}
