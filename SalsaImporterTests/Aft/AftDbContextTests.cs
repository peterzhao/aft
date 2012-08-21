using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;

namespace SalsaImporterTests
{
    [TestFixture]
    public class AftDbContextTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }
        [Test]
        public void ShouldCreateSupporterInAftDb()
        {
            var name = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter = new Supporter { Email = name + "@abc.com", First_Name = name, Last_Name = "Test"};
            Db(db => { db.Supporters.Add(supporter);
                         db.SaveChanges();
            });
            Assert.Greater(supporter.Id, 0);
        }

        [Category("IntegrationTest")]
        [Test]
        public void ShouldSetAndGetCustomFields()
        {
            var name1 = Guid.NewGuid().ToString().Substring(0, 6);
            var name2 = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter1 = new Supporter { Email = name1 + "@example.com", First_Name = name1, Last_Name = "Zhao" };
            var supporter2 = new Supporter { Email = name2 + "@example.com", First_Name = name2, Last_Name = "Zhao" };
            supporter1.CustomField["CustomString0"]= "Hi";
            supporter2.CustomField["CustomString0"]= "Good";

            Db(db => { 
                db.Supporters.Add(supporter1);
                db.SaveChanges();
            });

            Db(db => { //using different db contest to make sure SupporterCustomField not go to different contexts(use Id indead)
                db.Supporters.Add(supporter2);
                db.SaveChanges();
            });

            Assert.AreEqual("Hi", supporter1.CustomField["CustomString0"]);
            Assert.AreEqual("Good", supporter2.CustomField["CustomString0"]);

            Db(db =>
                   {
                       Assert.AreEqual("Hi", db.Supporters.Single(s => s.Email == supporter1.Email).CustomField["CustomString0"]);
                       Assert.AreEqual("Good", db.Supporters.Single(s => s.Email == supporter2.Email).CustomField["CustomString0"]);

                   });
        }

        [Category("IntegrationTest")]
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ShouldGetErrorWhenTryToSetCustomFieldWhichIsNotDefined()
        {
            var name = Guid.NewGuid().ToString().Substring(0, 6);
            var supporter = new Supporter { Email = name + "@example.com", First_Name = name, Last_Name = "Zhao" };
            supporter.CustomField["somthingNotDefined"] = "Hi";
        }


        public void Db(Action<AftDbContext> action)
        {
            using(var db = new AftDbContext())
            {
                action(db);
            }
        }
    }
}
