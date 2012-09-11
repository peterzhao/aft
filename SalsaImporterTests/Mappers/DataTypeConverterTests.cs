using System;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Exceptions;
using SalsaImporter.Mappers;

namespace SalsaImporterTests.Mappers
{
    [TestFixture]
    class DataTypeConverterTests
    {
  

        [Test]
        [ExpectedException(typeof(InvalidDataTypeException))]
        public void ShouldThrowOnInvalidDataType()
        {
            DataTypeConverter.GetConverter("baddatatype");
        }

        [Test]
        public void ShouldReadSalsaValueBoolean()
        {
            XElement element = XElement.Parse(@"<item>
                                                    <field>false</field>
                                                </item>");
            var converter = DataTypeConverter.GetConverter(DataType.Boolean);

            Assert.AreEqual(false, converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldReadSalsaValueDateTime()
        {
            XElement element = XElement.Parse(@"<item>
                                                    <field>Thu Aug 30 2012 11:19:43 GMT-0400 (EDT)</field>
                                                </item>");
            var converter = DataTypeConverter.GetConverter(DataType.DateTime);

            Assert.AreEqual(new DateTime(2012, 08, 30, 11, 19, 43), converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldGetConverterRegardlessCaseOfDataType()
        {
            XElement element = XElement.Parse(@"<item>
                                                    <field>Thu Aug 30 2012 11:19:43 GMT-0400 (EDT)</field>
                                                </item>");
            var converter = DataTypeConverter.GetConverter(DataType.DateTime);

            Assert.AreEqual(new DateTime(2012, 08, 30, 11, 19, 43), converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldReadSalsaValueFloat()
        {
            XElement element = XElement.Parse(@"<item>
                                                    <field>12.345</field>
                                                </item>");
            var converter = DataTypeConverter.GetConverter(DataType.Float);

            Assert.AreEqual(12.345f, converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldReadSalsaValueInt()
        {
            XElement element = XElement.Parse(@"<item>
                                                    <field>12345</field>
                                                </item>");
            var converter = DataTypeConverter.GetConverter(DataType.Int);

            Assert.AreEqual(12345, converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldReadSalsaValueString()
        {
            string expectedStringValue = "StringValue";
            XElement element = XElement.Parse(string.Format(@"<item>
                                                                <field>{0}</field>
                                                            </item>", expectedStringValue));
            var converter = DataTypeConverter.GetConverter("string");

            Assert.AreEqual(expectedStringValue, converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldMakeSalsaValueBoolean()
        {
            var converter = DataTypeConverter.GetConverter(DataType.Boolean);
            Assert.AreEqual("0", converter.MakeSalsaValue(false));
            Assert.AreEqual("1", converter.MakeSalsaValue(true));
        }

        
        [Test]
        public void ShouldMakeSalsaValueDateTime()
        {
            var converter = DataTypeConverter.GetConverter(DataType.DateTime);
            Assert.AreEqual("2001-02-03 04:56:00", converter.MakeSalsaValue(new DateTime(2001,2,3,4,56,00)));
        }

        [Test]
        public void ShouldMakeSalsaValueInt()
        {
            var converter = DataTypeConverter.GetConverter(DataType.Int);
            Assert.AreEqual("1234", converter.MakeSalsaValue(1234));
            Assert.AreEqual(null, converter.MakeSalsaValue(null));
        }

        [Test]
        public void ShouldMakeSalsaValueString()
        {
            var converter = DataTypeConverter.GetConverter(DataType.String);
            Assert.AreEqual("some value", converter.MakeSalsaValue("some value"));
        }

    }
}
