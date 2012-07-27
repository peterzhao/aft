using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SalsaImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var client = new SalsaClient();
            client.Authenticate();
            var count = client.Count();
            Console.WriteLine("total supporter on salsa:" + count);
            Console.ReadKey();
        }
    }
}
