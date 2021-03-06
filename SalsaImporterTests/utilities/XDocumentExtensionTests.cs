﻿
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    public class XDocumentExtensionTests
    {
        [Test]
        public void ShouldStringValueOfElement()
        {
            var xml = @"<item><Last_Name>zhao</Last_Name></item>";
            XElement element = XElement.Parse(xml);
            Assert.AreEqual("zhao", element.StringValueOrNull("Last_Name"));
            Assert.IsNull(element.StringValueOrNull("SomethingNotExist"));
        }

        [Test]
        public void ShouldIntValueOfElement()
        {
            var xml = @"<item><id>2345</id></item>";
            XElement element = XElement.Parse(xml);
            Assert.AreEqual(2345, element.IntValueOrNull("id"));
            Assert.IsNull(element.IntValueOrNull("SomethingNotExist"));
        }

        [Test]
        public void ShouldFloatValueOfElement()
        {
            var xml = @"<item><price>23.52</price></item>";
            XElement element = XElement.Parse(xml);
            Assert.AreEqual(23.52f, element.FloatValueOrNull("price"));
            Assert.IsNull(element.FloatValueOrNull("SomethingNotExist"));
        }
    }
}
