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
            var start = DateTime.Now;
            var tasks = new List<Task>();
           
            
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Factory.StartNew(wk =>
                {
                    var nameValues = new NameValueCollection();
                    nameValues.Add("xml", "");
                    nameValues.Add("object", "supporter");
                    nameValues.Add("Email", "mike" + i + "@abc.com");
                    nameValues.Add("key", "0");
                    nameValues.Add("First_Name", "Mike" + i);
                    nameValues.Add("Last_Name", "Handsome" + i);
                    client.PushObject(nameValues);
                }, null));
               
            }
            Task.WaitAll(tasks.ToArray());
            var pushEnd = DateTime.Now;
            client.PullObejcts();
            var pullEnd = DateTime.Now;

            Console.WriteLine("Push spent(ms):" + (pushEnd - start).TotalMilliseconds);
            Console.WriteLine("Pull spent(ms):" + (pullEnd - pushEnd).TotalMilliseconds);
            Console.ReadKey();
        }
    }
}
