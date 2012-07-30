using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SalsaImporter
{
    public class SalsaClient
    {
        private readonly CookieContainer cookies = new CookieContainer();
        private readonly string salsaUrl;

        public SalsaClient()
        {
            salsaUrl = Config.SalsaApiUri;
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
        }

        public void Authenticate()
        {
            Logger.Debug("authenticating...");
            var url = salsaUrl;
            var username = Config.SalsaUserName;
            var password = Config.SalsaPassword;
            var response = ExtentedWebClient.Try(() =>
                                                     {
                                                         var webRequest =
                                                             WebRequest.Create(url + "api/authenticate.sjs?email=" +
                                                                               username + "&password=" + password);
                                                         ((HttpWebRequest) webRequest).CookieContainer = cookies;
                                                         webRequest.Method = "POST";
                                                         webRequest.ContentLength = 0;
                                                         webRequest.ContentType = "application/x-www-form-urlencoded";
                                                         return (HttpWebResponse) webRequest.GetResponse();
                                                     }, 3);


            Logger.Debug("response: " + response);
        }


        public void EachBatchOfObjects(string objectType,
                                    int blockSize,
                                    Action<List<XElement>> batchHandler,
                                    IEnumerable<String> fieldsToReturn = null)
        {
            int start = 0;
            for (;;) // ever
            {
                string url =
                    String.Format(
                        "{0}api/getObjects.sjs?object={1}&limit={2},{3}",
                        salsaUrl, objectType, start, blockSize);

                if (fieldsToReturn != null)
                {
                    url += "&include=" + String.Join(",", fieldsToReturn);
                }

                Logger.Debug("Requesting: " + url);
                string response = ExtentedWebClient.Try(() =>
                                              {
                                                  using (var webClient = new ExtentedWebClient(cookies))
                                                  {
                                                      string result = webClient.DownloadString(url);
                                                      return result;
                                                  }
                                              }, 3);
                                                
                Logger.Debug("response from PullObjects: " + response);

                List<XElement> supporters = XDocument.Parse(response).Descendants("item").ToList();

                if (supporters.Count == 0)
                    break;

                batchHandler(supporters);
                start += blockSize;
                if (supporters.Count < blockSize)
                    break;
            }
        }

        public void DeleteAllObjects(string objectType, int blockSize)
        {
            int start = 0;
            Logger.Info(String.Format("Deleting all {0}s...", objectType));
            for (; ; ) // ever
            {
                //Authenticate();

                string url = String.Format("{0}api/getObjects.sjs?object={1}&limit={2},{3}&include={1}_KEY",
                        salsaUrl, objectType, start, blockSize);
                string response = Get(url);


                XDocument responseXml = XDocument.Parse(response);
                
                if(responseXml.Descendants("error").Any())
                {
                    Logger.Warn(string.Format("SalsaClient.DeleteAllObjects got error and skipped. {0}", responseXml.Descendants("error").First().Value));
                    Authenticate();
                    continue;
                }
                List<XElement> items = responseXml.Descendants("item").ToList();

                if (items.Count == 0) break;

                DeleteObjects(objectType, items.Select(s => s.Element("key").Value));
                if (items.Count < blockSize) break;
            }
            Logger.Info(String.Format("All {0}s deleted.", objectType));
        }

        private string Get(string url)
        {
            Logger.Debug("Requesting: " + url);

            string response = ExtentedWebClient.Try(() =>
                                                    {
                                                        using (var webClient = new ExtentedWebClient(cookies))
                                                        {
                                                            string result = webClient.DownloadString(url);
                                                            return result;
                                                        }
                                                    }, 5);
            Logger.Debug("response from PullObjects: " + response);
            return response;
        }

        public void DeleteObject(string objectType, string key)
        {
            var data = new NameValueCollection {{"key", key}};
            Post("delete", objectType, data);
        }

        public string CreateSupporter(NameValueCollection supporter)
        {
            return Create("supporter", supporter);
        }

        public int SupporterCount()
        {
            return CountObjects("supporter");
        }

        public XElement GetSupporter(string key)
        {
            return GetObject(key, "supporter");
        }

        public void CreateSupporters(IEnumerable<NameValueCollection> supporters)
        {
            IEnumerable<Task> tasks = supporters.Select(supporter =>
                                                        Task.Factory.StartNew(arg =>
                                                        {
                                                            var nameValues = (NameValueCollection)arg;
                                                                var id = CreateSupporter(nameValues);
                                                                nameValues["supporter_KEY"] = id;
                                                        }, supporter));
            try
            {
                Task.WaitAll(tasks.ToArray(), -1); //no timeout
            }catch(AggregateException ex)
            {
                var message = "";
                ex.InnerExceptions.ToList().ForEach(e => message += ex.ToString() + "/n");
                throw new ApplicationException(string.Format("SalsaClient.CreateSupporters got {0} error(s): /n{1}", ex.InnerExceptions.Count, message));
            }
        }

        public void DeleteObjects(string objectType, IEnumerable<string> keys)
        {
            Logger.Info("Deleting objects");
            IEnumerable<Task> tasks = keys.Select(supporterKey =>
                                                  Task.Factory.StartNew(wk => DeleteObject(objectType, supporterKey),
                                                                        null));
            try
            {
                Task.WaitAll(tasks.ToArray(), -1); //no timeout
            }
            catch (AggregateException ex)
            {
                var message = "";
                ex.InnerExceptions.ToList().ForEach(e => message += ex.ToString() + "/n");
                throw new ApplicationException(string.Format("SalsaClient.DeleteObjects got {0} error(s): /n{1}", ex.InnerExceptions.Count, message));
            }
        }

        public int CustomColumnCount()
        {
            return CountObjects("custom_column");
        }

        public string CreateSupporterCustomColumn(NameValueCollection customField)
        {
            customField.Set("database_table_KEY", "142"); // Not sure what this is, but neccesary
            customField.Set("data_table", "supporter_custom");  // Custom field table for supporters
            return Create("custom_column", customField);
        }

        private string Create(string objectType, NameValueCollection customField)
        {
            customField.Set("key", "0"); // this is to indicate creation   
            string response = Post("save", objectType, customField);
            return XDocument.Parse(response).Element("data").Element("success").Attribute("key").Value;
        }

        public XElement GetCustomColumn(string key)
        {
            return GetObject(key, "custom_column");
        }

        private int CountObjects(string objectType)
        {
            return ExtentedWebClient.Try(() =>
                                      {
                                          using (var client = new ExtentedWebClient(cookies))
                                          {
                                              Logger.Debug("Counting Objects...");
                                              string url = salsaUrl +
                                                           string.Format(
                                                               "api/getCount.sjs?object={0}&countColumn={0}_KEY",
                                                               objectType);
                                              string result = client.DownloadString(url);
                                              Logger.Debug("response: " + result);
                                              XDocument xml = XDocument.Parse(result);
                                              string value = xml.Descendants("count").First().Value;
                                              int countObjects = Int32.Parse(value);
                                              return countObjects;
                                          }
                                      }, 3);
        }

        private XElement GetObject(string key, string objectType)
        {
            return ExtentedWebClient.Try(() =>
                                             {
                                                 XElement xElement;
                                                using (var client = new ExtentedWebClient(cookies))
                                                {
                                                    Logger.Debug(string.Format("Getting {0} {1}...",
                                                                                objectType, key));
                                                    string url =
                                                        String.Format(
                                                            "{0}api/getObject.sjs?object={1}&key={2}",
                                                            salsaUrl, objectType, key);
                                                    string result = client.DownloadString(url);
                                                    Logger.Debug("response: " + result);
                                                    XDocument xml = XDocument.Parse(result);
                                                    xElement =
                                                        xml.Element("data").Element(objectType).Element(
                                                            "item");
                                                }
                                                return xElement;
                                            }, 3);
        }

        private string Post(string action, string objectType, NameValueCollection data)
        {
            return ExtentedWebClient.Try(() =>
                                             {
                                                 string response1;
                                                 using (var client1 = new ExtentedWebClient(cookies))
                                                 {
                                                     Logger.Debug(string.Format("POST to {0} {1} with {2}",
                                                                                action, objectType, data));
                                                     data.Set("xml", "");
                                                     data.Set("object", objectType);

                                                     string url1 = salsaUrl + action;
                                                     byte[] result1 = client1.UploadValues(url1, "POST", data);
                                                     response1 = Encoding.UTF8.GetString(result1);
                                                     Logger.Debug("response: " + response1);
                                                 }
                                                 return response1;
                                             }, 5);
        }

    }
}