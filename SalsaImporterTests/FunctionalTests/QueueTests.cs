using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Salsa;

namespace SalsaImporterTests.FunctionalTests
{
    [TestFixture]
    public class QueueTests
    {
        public QueueTests()
        {
            Config.Environment = Config.Test;
        }

        [Test]
        public void ShouldInsertToQueue()
        {
            SalsaClient client = new SalsaClient();
            string id = client.Create("supporter", new NameValueCollection
                                                       {
                                                           {"Email", "bob@abc.com"},
                                                           {"First_Name", "First"},
                                                           {"Last_Name", "Last"}
                                                       });
            XElement supporter = client.GetObject("supporter", id);

            var fields = new List<string> { "First_Name", "Last_Name", "Email" };

            var dbContext = new AftDbContext();
            dbContext.InsertToQueue(supporter, "Supporter_SalsaToAftQueue", fields);
        }

    }


}