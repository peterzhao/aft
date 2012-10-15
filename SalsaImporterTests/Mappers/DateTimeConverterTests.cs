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
    public class DateTimeConverterTests
    {
       

        private DataTypeConverter converter;
        [SetUp]
        public void SetUp()
        {
            converter = DataTypeConverter.GetConverter(DataType.DateTime);
            Assert.IsTrue(converter is DateTimeConverter);

        }
       
        [Test]
        public void ShouldGetNullWhenReadDbNullFromAFT()
        {
            Assert.IsNull(converter.ReadAftValue(DBNull.Value));
        }

        [Test]
        public void ShouldGetValueFromAFT()
        {
            var dateTime = new DateTime(2012, 8, 23, 23, 30, 25, 112);
            var expectedDateTime = new DateTime(2012, 8, 23, 23, 30, 25);

            Assert.AreEqual(expectedDateTime, converter.ReadAftValue(dateTime));
        }

        [Test]
        public void ShouldGetValueFromSalsa()
        {
            var xml = XElement.Parse("<item><value>Wed Oct 03 2012 10:55:42 GMT-0400 (EDT)</value></item>");
            Assert.AreEqual(new DateTime(2012, 10, 3, 10, 55, 42), converter.ReadSalsaValue("value", xml));
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
            Assert.AreEqual("2012-10-03 10:55:42", converter.MakeSalsaValue(new DateTime(2012, 10, 3, 10, 55, 42)));
        }

        [Test]
        public void ShouldMakeSpaceForSalsa()
        {
            Assert.AreEqual(" ", converter.MakeSalsaValue(null));
        }
    }
}
