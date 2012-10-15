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
    public class IntConverterTests
    {
        private DataTypeConverter converter;
        [SetUp]
        public void SetUp()
        {
            converter = DataTypeConverter.GetConverter(DataType.Int);
            Assert.IsTrue(converter is IntConverter);

        }

        [Test]
        public void ShouldGetValueFromAFT()
        {
            Assert.AreEqual(25, converter.ReadAftValue(25));
        }
        [Test]
        public void ShouldConvert0ToNullFromAFT()
        {
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
            var xml = XElement.Parse("<item><intValue>0</intValue></item>");
            Assert.IsNull(converter.ReadSalsaValue("intValue",xml ));
        }

        [Test]
        public void ShouldGetNullFromSalsa()
        {
            var xml = XElement.Parse("<item></item>");
            Assert.IsNull(converter.ReadSalsaValue("intValue", xml));
        }

        [Test]
        public void ShouldGetValueFromSalsa()
        {
            var xml = XElement.Parse("<item><intValue>34</intValue></item>");
            Assert.AreEqual(34, converter.ReadSalsaValue("intValue", xml));
        }

        [Test]
        public void ShouldMakeStringForSalsa()
        {
            Assert.AreEqual("45", converter.MakeSalsaValue(45));
        }

        [Test]
        public void ShouldMake0ForNullValueForSalsa()
        {
            Assert.AreEqual("0", converter.MakeSalsaValue(null));
        }



    }
}
