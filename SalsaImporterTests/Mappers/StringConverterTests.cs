using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Mappers;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    public class StringConverterTests
    {
        private DataTypeConverter converter;
        [SetUp]
        public void SetUp()
        {
            converter = DataTypeConverter.GetConverter(DataType.String);
            Assert.IsTrue(converter is StringConverter);

        }
        [Test]
        public void ShouldTrimSpacesWhenReadValueFromAFT()
        {
            Assert.AreEqual("zhao", converter.ReadAftValue("   zhao   "));
        }

        [Test]
        public void ShouldGetNullWhenReadDbNullFromAFT()
        {
            Assert.IsNull(converter.ReadAftValue(DBNull.Value));
        }

        [Test]
        public void ShouldGetNullWhenEmptyFromAFT()
        {
            Assert.IsNull(converter.ReadAftValue(""));
            Assert.IsNull(converter.ReadAftValue(" "));
        }

        [Test]
        public void ShouldGetValueFromSalsa()
        {
            var xml = XElement.Parse("<item><value>Foo</value></item>");
            Assert.AreEqual("Foo", converter.ReadSalsaValue("value", xml));
        }

        [Test]
        public void ShouldGetNullFromSalsa()
        {
            var xml = XElement.Parse("<item></item>");
            Assert.IsNull(converter.ReadSalsaValue("value", xml));
        }

        [Test]
        public void ShouldMakeStringForSalsa()
        {
            Assert.AreEqual("Foo", converter.MakeSalsaValue("Foo"));
        }

        [Test]
        public void ShouldMakeSpaceForSalsa()
        {
            Assert.AreEqual(" ", converter.MakeSalsaValue(null));
            Assert.AreEqual(" ", converter.MakeSalsaValue(""));
        }



    }
}
