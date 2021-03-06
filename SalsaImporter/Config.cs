﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using SalsaImporter.Utilities;

namespace SalsaImporter
{
    public class Config
    {
        public static string Test = "test";
        public static string PerformanceTest = "performanceTest";
        public static string Production = "production";
        public static string Demo = "demo";
        public static string Uat = "Uat";
        public static string Stub = "stub";
        private static string _environment;

        private Config()
        {
        }


        public static string Environment
        {
            get { return _environment; }
            set
            {
                _environment = value;
                SetDbConnectionForLogging();
            }
        }

        public static bool SalsaWritable
        {
            get { return IsSettingPresent("salsaWritable"); }
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

        public static bool DbTrustedConnection
        {
            get { return GetSetting("dbTrustedConnection").EqualsIgnoreCase("True"); }
        }

        public static string DbConnectionString
        {
            get
            {
                if (DbTrustedConnection)
                    return String.Format("Server={0};Database={1};Trusted_Connection=true;Connection Timeout=120", GetSetting("dbHost"), GetSetting("dbName"));

                return string.Format("Server={0};Database={1};User Id={2};Password={3};Connection Timeout=120",
                        GetSetting("dbHost"), GetSetting("dbName"), GetSetting("dbUserName"), GetSetting("dbPassword"));
            }
        }

        private static IEnumerable<string> Environments
        {
            get { return XDocument.Load("environments.xml").Element("environments").Elements().Select(e => e.Name.LocalName); }
        }

        public static string SmtpFromAddress
        {
            get { return GetSetting("smtpFromAddress"); }
        }

        public static string SmtpNotificationRecipient
        {
            get { return GetSetting("smtpNotificationRecipient"); }
        }

        public static string SmtpHost
        {
            get { return GetSetting("smtpHost"); }
        }

        public static int SmtpPort
        {
            get { return int.Parse(GetSetting("smtpPort")); }
        }

        public static bool SmtpRequireSsl
        {
            get { 
                string setting = GetSetting("smtpRequireSsl");
                if (!String.IsNullOrEmpty(setting) && setting.ToLower() == "true")
                {
                    return true;
                }
                return false;
            }
        }

        public static string SmtpFromPassword
        {
            get { return GetSetting("smtpFromPassword"); }
        }

        public static string SmtpUserName
        {
            get { return GetSetting("smtpUserName"); }
        }

        public static bool SmtpRequireLogin
        {
            get { 
                string setting = GetSetting("smtpRequireLogin");
                if(!String.IsNullOrEmpty(setting) && setting.ToLower()=="true")
                {
                    return true;
                }
                return false;
            }
        }

        public static int BatchSize
        {
            get { return int.Parse(GetSetting("batchSize")); }
        }

        public static int ErrorToleranceThreshold
        {
            get { return int.Parse(GetSetting("errorToleranceThreshold")); }
        }

        private static void SetDbConnectionForLogging()
        {
            const string targetName = "db";
            var loggingConfiguration = LogManager.Configuration;
            var target = loggingConfiguration.AllTargets.ToList().FirstOrDefault(t => t.Name == targetName) as DatabaseTarget;
            if (target == null)
            {
                Logger.Warn("Could not find Nlog database target 'db' in NLog.config file. Not database logging is available.");
                return;
            }
                
            target.ConnectionString = DbConnectionString;
            
            LogManager.ReconfigExistingLoggers();
        }

        private static bool IsSettingPresent(string name)
        {
            var xElement = GetEnvironmentsElement(name);
            return (xElement != null);
        }

        private static string GetSetting(string name)
        {
            var xElement = GetEnvironmentsElement(name);
            if (xElement == null) return System.Environment.GetEnvironmentVariable(name);
            return xElement.Value;
        }

        private static XElement GetEnvironmentsElement(string name)
        {
            XDocument root = XDocument.Load("environments.xml");
            XElement envRoot = root.Element("environments").Element(Environment);
            if (envRoot == null)
            {
                throw new ApplicationException(
                    string.Format("Cannot find environment {0} in environment.xml. Available environments: {1}", Environment,
                                  string.Join(",", Environments)));
            }
            var xElement = envRoot.Element(name);
            return xElement;
        }

        
    }
}