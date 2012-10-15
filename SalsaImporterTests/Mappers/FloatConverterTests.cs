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
    public class FloatConverterTests
    {
        private DataTypeConverter converter;
        [SetUp]
        public void SetUp()
        {
            converter = DataTypeConverter.GetConverter(DataType.Float);
            Assert.IsTrue(converter is FloatConverter);

        }

        [Test]
        public void ShouldGetValueFromAFT()
        {
            Assert.AreEqual(25.05, converter.ReadAftValue(25.05));
            Assert.AreEqual(25, converter.ReadAftValue(25));
        }
        [Test]
        public void ShouldConvert0ToNullFromAFT()
        {
            Assert.IsNull(converter.ReadAftValue(0.00));
            Assert.IsNull(converter.ReadAftValue(0.0));
            Assert.IsNull(converter.ReadAftValue(0));
        }

        [Test]
        public void ShouldGetNullWhenReadDbNullFromAFT()
        {
            Assert.IsNull(converter.ReadAftValue(DBNull.Value));
        }


        [Test]
        public void ShouldConvert0ToNullFromSalsa()
        {
            var xml = XElement.Parse("<item><value>0.00</value></item>");
            Assert.IsNull(converter.ReadSalsaValue("value", xml));
        }

        [Test]
        public void ShouldGetNullFromSalsa()
        {
            var xml = XElement.Parse("<item></item>");
            Assert.IsNull(converter.ReadSalsaValue("value", xml));
        }

        [Test]
        public void ShouldGetValueFromSalsa()
        {
            var xml = XElement.Parse("<item><value>34.56</value></item>");
            Assert.AreEqual(34.56f, converter.ReadSalsaValue("value", xml));
        }

        [Test]
        public void ShouldMakeStringForSalsa()
        {
            Assert.AreEqual("45.78", converter.MakeSalsaValue(45.78f));
        }

        [Test]
        public void ShouldMake0ForNullValueForSalsa()
        {
            Assert.AreEqual("0.0", converter.MakeSalsaValue(null));
        }
    }
}
