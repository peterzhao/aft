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
            Console.WriteLine(string.Format("{0} {1}", DateTime.Now, message));
        }
    }
}
