using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter
{
    public class Logger
    {
        private static NLog.Logger nLogger = NLog.LogManager.GetLogger("Importer");

        public static void Debug(string message)
        {
           nLogger.Debug(message);
        }

        public static void Info(string message)
        {
            nLogger.Info(message);
        }

        public static void Warn(string message)
        {
            nLogger.Warn(message);
        }

        public static void Error(string message, Exception e = null)
        {
            nLogger.Error(string.Format("{0} {1} {2}", message, e.Message, e.StackTrace), e);
        }

        public static void Fatal(string message, Exception e = null)
        {
            nLogger.Fatal(string.Format("{0} {1} {2}", message, e.Message, e.StackTrace), e);
        }
    }
}
