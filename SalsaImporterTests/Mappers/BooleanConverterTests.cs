using System;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Mappers;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    public class BooleanConverterTests
    {

        [SetUp]
        public void SetUp()
        {
            converter = DataTypeConverter.GetConverter(DataType.Boolean);
            Assert.IsTrue(converter is BooleanConverter);
        }


        private DataTypeConverter converter;

        [Test]
        public void ShouldGetFalseFromSalsa()
        {
            XElement xml = XElement.Parse("<item></item>");
            Assert.AreEqual(false, converter.ReadSalsaValue("value", xml));
        }


        [Test]
        public void ShouldGetFalseWhenReadDbNullFromAFT()
        {
            Assert.AreEqual(false, converter.ReadAftValue(DBNull.Value));
        }


        [Test]
        public void ShouldGetValueFromSalsa()
        {
            XElement xml = XElement.Parse("<item><value>true</value></item>");
            Assert.AreEqual(true, converter.ReadSalsaValue("value", xml));
            XElement xml2 = XElement.Parse("<item><value>false</value></item>");
            Assert.AreEqual(false, converter.ReadSalsaValue("value", xml2));
        }

        [Test]
        public void ShouldGetValueWhenReadFromAFT()
        {
            Assert.AreEqual(true, converter.ReadAftValue(true));
        }

        [Test]
        public void ShouldMakeStringForSalsa()
        {
            Assert.AreEqual("1", converter.MakeSalsaValue(true));
            Assert.AreEqual("0", converter.MakeSalsaValue(false));
        }
    }
}