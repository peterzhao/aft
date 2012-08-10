using System;
using System.Collections.Generic;
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
        public static string Test = "test";
        public static string PerformanceTest = "performanceTest";
        public static string Production = "production";
        public static string Demo = "demo";
        public static string Stub = "stub";
        private static string _environment;

        private Config()
        {
            Environment = Test;
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

        private static IEnumerable<string> Environments
        {
            get { return XDocument.Load("environments.xml").Element("environments").Elements().Select(e => e.Name.LocalName); }
        }

        private static void AddDbTargetToNLog()
        {
            string name = "db";
            LoggingConfiguration loggingConfiguration = LogManager.Configuration;
            if (loggingConfiguration.AllTargets.ToList().Any(t => t.Name == name))
            {
                loggingConfiguration.RemoveTarget(name);
            }
            var target = new DatabaseTarget
                             {
                                 Name = name, ConnectionString = DbConnectionString, KeepConnection = true, UseTransactions = true,
                                 CommandText = "INSERT INTO ImporterLogs([time_stamp], [level], [threadId], [message], [exception]) VALUES (@time_stamp,@level,@threadid,@message,@exception)"
                             };
            target.Parameters.Add(new DatabaseParameterInfo("@time_stamp", new SimpleLayout("${date}")));
            target.Parameters.Add(new DatabaseParameterInfo("@level", new SimpleLayout("${level}")));
            target.Parameters.Add(new DatabaseParameterInfo("@threadid", new SimpleLayout("${threadid}")));
            target.Parameters.Add(new DatabaseParameterInfo("@message", new SimpleLayout("${message}")));
            target.Parameters.Add(new DatabaseParameterInfo("@exception", new SimpleLayout("${exception:format=type,message,stacktrace:maxInnerExceptionLevel=5:innerFormat=shortType,message,method}")));

            loggingConfiguration.AddTarget(name, target);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            LogManager.ReconfigExistingLoggers();
        }

        private static string GetSetting(string name)
        {
            XDocument root = XDocument.Load("environments.xml");
            XElement envRoot = root.Element("environments").Element(Environment);
            if (envRoot == null)
            {
                throw new ApplicationException(string.Format("Cannot find environment {0} in environment.xml. Available environments: {1}", Environment, string.Join(",", Environments)));
            }
            var xElement = envRoot.Element(name);
            if (xElement == null) return System.Environment.GetEnvironmentVariable(name);
            return xElement.Value;
        }
    }
}