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
            if (e != null)
                nLogger.ErrorException(message, e);
            else
                nLogger.Error(message);
        }

        public static void Fatal(string message, Exception e = null)
        {
            if (e != null)
                nLogger.FatalException(message, e);
            else
                nLogger.Fatal(message);
        }

        public static void Trace(string message)
        {
            nLogger.Trace(message);
        }
    }
}
