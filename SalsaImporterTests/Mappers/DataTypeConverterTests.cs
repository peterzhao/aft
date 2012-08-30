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
    class DataTypeConverterTests
    {
        [Test]
        public void ShouldGetFieldValueAsString()
        {
            string expectedStringValue = "StringValue";
            XElement element = XElement.Parse(string.Format(@"<item>
                                                                <field>{0}</field>
                                                            </item>", expectedStringValue));
            var converter = DataTypeConverter.GetConverter("string");

            Assert.AreEqual(expectedStringValue, converter.ReadSalsaValue("field", element));
        }

        [Test]
        public void ShouldGetFieldValueAsDateTime()
        {
            XElement element = XElement.Parse(@"<item>
                                                    <field>Thu Aug 30 2012 11:19:43 GMT-0400 (EDT)</field>
                                                </item>");
            var converter = DataTypeConverter.GetConverter("datetime");

            Assert.AreEqual(new DateTime(2012, 08, 30, 11, 19, 43), converter.ReadSalsaValue("field", element));
        }


    }
}
