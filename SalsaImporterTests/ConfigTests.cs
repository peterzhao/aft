using System;
using NUnit.Framework;
using SalsaImporter;

namespace SalsaImporterTests
{
    [TestFixture]
    public class ConfigTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
        }

        [Test]
        public void ShouldGetDbConnectionString()
        {
            Assert.AreEqual("Server=.;Database=Aft;Trusted_Connection=true;Connection Timeout=120", Config.DbConnectionString);
        }

        [Test]
        public void ShouldGetDbConnectionStringForNonTrustedConnection()
        {
            Config.Environment = Config.Production;
            Environment.SetEnvironmentVariable("dbUserName", "sa");
            Environment.SetEnvironmentVariable("dbPassword", "1234");
            Assert.IsTrue(Config.DbConnectionString.Contains("User Id=sa"));
            Config.Environment = Config.Test;
        }

        [Test]
        public void ShouldGetConfigForDifferentEnvironments()
        {
            Config.Environment = Config.Demo;
            Assert.AreEqual("aftsalsademo@example.com", Config.SalsaUserName);
            Assert.AreEqual("https://sandbox.salsalabs.com/", Config.SalsaApiUri);
            Assert.IsNotNull(Config.SalsaPassword);

            Config.Environment = Config.PerformanceTest;
            Assert.AreEqual("tom@alexandrowicz.ca", Config.SalsaUserName);
        }

        [Test]
        public void ShouldGetUserNameAndPasswordFromEnvironmentVariablesIfTheyCannotBeFoundInConfigFile()
        {
            Config.Environment = Config.Test;
            Assert.AreEqual("https://sandbox.salsalabs.com/", Config.SalsaApiUri);
            Assert.AreEqual(Environment.GetEnvironmentVariable("salsaPassword"), Config.SalsaPassword);
            Assert.IsNotNull(Config.SalsaPassword);
            Assert.AreEqual(Environment.GetEnvironmentVariable("salsaUserName"), Config.SalsaUserName);
            Assert.IsNotNull(Config.SalsaUserName);
        }
    }
}