using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests.Aft
{
    public class SupporterCustomFieldTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }
        [Test]
        [Category("IntegrationTest")]
        public void ShouldGetAllFeildsFromDb()
        {
            var fields = SupporterCustomField.All;
            Assert.IsTrue(fields.Any(f => f.Name == "CustomString0"));
            Assert.IsTrue(fields.Any(f => f.Name == "CustomString9"));
            Assert.IsTrue(fields.Any(f => f.Name == "CustomBoolean0"));
            Assert.IsTrue(fields.Any(f => f.Name == "CustomBoolean4"));
            Assert.IsTrue(fields.Any(f => f.Name == "CustomDateTime0"));
        }
    }
}
