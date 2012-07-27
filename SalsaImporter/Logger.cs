using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter
{
    public class Logger
    {
     
        public static void Debug(string message)
        {
            Console.WriteLine(DateTime.Now + " " + message);
        }

        public static void Info(string message)
        {
            Console.WriteLine(DateTime.Now + " " + message);
        }

        public static void Warn(string message)
        {
            Console.WriteLine(DateTime.Now + " " + message);
        }

        public static void Error(string message, Exception e = null)
        {
            Console.WriteLine(DateTime.Now + " " + message);
        }

        public static void Fatal(string message, Exception e = null)
        {
            Console.WriteLine(DateTime.Now + " " + message);
        }
    }
}
