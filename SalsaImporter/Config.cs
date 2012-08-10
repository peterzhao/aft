using System.Linq;
using System.Xml.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace SalsaImporter
{
    public class Config
    {
        public static string UnitTest = "test";
        public static string PerformanceTest = "performanceTest";
        public static string Production = "production";
        public static string Dev = "dev";
        public static string Demo = "demo";
        public static string Stub = "stub";
        private static string _environment;

        Config()
        {
            Config.Environment = Config.Dev;
        }


        public static string Environment
        {
            get { return _environment; }
            set
            {
                _environment = value;
                AddDbTargetToNLog();
            }
        }

        private static void AddDbTargetToNLog()
        {
            var name = "db";
            var loggingConfiguration = NLog.LogManager.Configuration;
            if (loggingConfiguration.AllTargets.ToList().Any(t => t.Name == name))
            {
                loggingConfiguration.RemoveTarget(name);

            }
            var target = new DatabaseTarget {Name = name, ConnectionString = Config.DbConnectionString, KeepConnection = true, UseTransactions = true, 
                CommandText = "INSERT INTO ImporterLogs([time_stamp], [level], [threadId], [message], [exception]) VALUES (@time_stamp,@level,@threadid,@message,@exception)"};
            target.Parameters.Add(new DatabaseParameterInfo("@time_stamp", new SimpleLayout("${date}")));
            target.Parameters.Add(new DatabaseParameterInfo("@level", new SimpleLayout("${level}")));
            target.Parameters.Add(new DatabaseParameterInfo("@threadid", new SimpleLayout("${threadid}")));
            target.Parameters.Add(new DatabaseParameterInfo("@message", new SimpleLayout("${message}")));
            target.Parameters.Add(new DatabaseParameterInfo("@exception", new SimpleLayout("${exception:format=type,message,stacktrace:maxInnerExceptionLevel=5:innerFormat=shortType,message,method}")));
            
            loggingConfiguration.AddTarget(name, target);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            NLog.LogManager.ReconfigExistingLoggers();
        }

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