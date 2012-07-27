using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter
{
    public class Logger
    {
        private static readonly log4net.ILog Log;
        static Logger()
        {
            //log4net.Config.BasicConfigurator.Configure();
            //Log = log4net.LogManager.GetLogger(typeof(Program));           
 
        }
        public static void Debug(string message)
        {
            //Log.Debug(message);
        }

        public static void Info(string message)
        {
           // Log.Info(message);
            Console.WriteLine(DateTime.Now + " " + message);
        }

        public static void Warn(string message)
        {
            //Log.Warn(message);
        }

        public static void Error(string message, Exception e = null)
        {
            //Log.Error(message, e);
        }

        public static void Fatal(string message, Exception e = null)
        {
            //Log.Fatal(message, e);
        }
    }
}
