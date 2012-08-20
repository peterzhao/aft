using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Salsa;

namespace SalsaImporterTests.Utilities
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class CreateTestingCustomColumnsTests
    {
        public CreateTestingCustomColumnsTests()
        {
            Config.Environment = Config.Test;
        }


        [Test]
        public void ShouldCreateCustomColumnsAndReturnThemEmptyOnNewSupporter()
        {
            new CreateTestingCustomColumns().CreateCustomColumns(new List<SupporterCustomColumnsRequest>()
                                                                     {
                                                                         new SupporterCustomColumnsRequest
                                                                             {CustomColumnName = "String", HowManyToMake = 10, SalsaType = "varchar"},
                                                                         new SupporterCustomColumnsRequest
                                                                             {CustomColumnName = "Boolean", HowManyToMake = 10, SalsaType = "bool"},
                                                                         new SupporterCustomColumnsRequest
                                                                             {CustomColumnName = "Integer", HowManyToMake = 5, SalsaType = "int"},
                                                                         new SupporterCustomColumnsRequest
                                                                             {CustomColumnName = "DateTime", HowManyToMake = 1, SalsaType = "datetime"}
                                                                     });

            var salsaClient = new SalsaClient();

            string newSupporterId = salsaClient.Create("supporter", new NameValueCollection {{"email", "customcolumn@test.com"}});

            XElement newSupporterXml = salsaClient.GetObject("supporter", newSupporterId);

            Assert.NotNull(newSupporterXml.Element("CustomString0"));
            Assert.NotNull(newSupporterXml.Element("CustomString1"));
            Assert.NotNull(newSupporterXml.Element("CustomString2"));
            Assert.NotNull(newSupporterXml.Element("CustomString3"));
            Assert.NotNull(newSupporterXml.Element("CustomString4"));

            Assert.NotNull(newSupporterXml.Element("CustomString5"));
            Assert.NotNull(newSupporterXml.Element("CustomString6"));
            Assert.NotNull(newSupporterXml.Element("CustomString7"));
            Assert.NotNull(newSupporterXml.Element("CustomString8"));
            Assert.NotNull(newSupporterXml.Element("CustomString9"));

            Assert.NotNull(newSupporterXml.Element("CustomBoolean0"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean1"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean2"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean3"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean4"));

            Assert.NotNull(newSupporterXml.Element("CustomBoolean5"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean6"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean7"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean8"));
            Assert.NotNull(newSupporterXml.Element("CustomBoolean9"));

            Assert.NotNull(newSupporterXml.Element("CustomInteger0"));
            Assert.NotNull(newSupporterXml.Element("CustomInteger1"));
            Assert.NotNull(newSupporterXml.Element("CustomInteger2"));
            Assert.NotNull(newSupporterXml.Element("CustomInteger3"));
            Assert.NotNull(newSupporterXml.Element("CustomInteger4"));

            Assert.NotNull(newSupporterXml.Element("CustomDateTime0"));
        }
    }
}