using System.Xml.Linq;

namespace SalsaImporter
{
    public class Config
    {
        public static string UnitTest = "test";
        public static string PerformanceTest = "performanceTest";
        public static string Production = "production";
        public static string Dev = "dev";
        public static string Stub = "stub";
        public static string Environment = Config.Dev;

        public static string SalsaApiUri
        {
            get { return GetSetting("salsaApiUrl"); }
        }

        public static string SalsaUserName
        {
            get { return GetSetting("salsaUserName"); }
        }

        public static string SalsaPassword
        {
            get { return GetSetting("salsaPassword"); }
        }

        public static string DbConnectionString
        {
            get { return GetSetting("dbConnectionString"); }
        }

        private static string GetSetting(string name)
        {
            var root = XDocument.Load("environments.xml");
            return root.Element("environments").Element(Environment).Element(name).Value;
        }
    }
}