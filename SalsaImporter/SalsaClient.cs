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
                                                         webRequest.ContentType = "application/x-www-form-urlencoded";
                                                         return (HttpWebResponse) webRequest.GetResponse();
                                                     }, 3);


            Logger.Debug("response: " + response);
        }


        public void SalsaGetObjects(string objectType,
                                    int blockSize,
                                    Action<List<XElement>> batchHandler,
                                    IEnumerable<String> fieldsToReturn = null)
        {
            int start = 0;
            for (;;) // ever
            {
                var webClient = new ExtentedWebClient(cookies, 30000);
                string url =
                    String.Format(
                        "{0}api/getObjects.sjs?object={1}&limit={2},{3}",
                        salsaUrl, objectType, start, blockSize);

                if (fieldsToReturn != null)
                {
                    url += "&include=" + String.Join(",", fieldsToReturn);
                }

                Logger.Debug("Requesting: " + url);
                string response = ExtentedWebClient.Try(() => webClient.DownloadString(url), 3);
                Logger.Debug("response from PullObjects: " + response);

                List<XElement> supporters = XDocument.Parse(response).Descendants("item").ToList();

                if (supporters.Count == 0)
                    break;

                batchHandler(supporters);

                if (supporters.Count < blockSize)
                    break;
            }
        }

        public void DeleteObject(string objectType, string key)
        {
            var data = new NameValueCollection {{"key", key}};
            SalsaPost("delete", objectType, data);
        }


        public string SaveSupporter(NameValueCollection data)
        {
            string response;
            response = SalsaPost("save", "supporter", data);
            return XDocument.Parse(response).Element("data").Element("success").Attribute("key").Value;
        }

        public int SupporterCount()
        {
            return CountObjects("supporter");
        }

        public XElement GetSupporter(string key)
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Getting supporter ...");

                string url = String.Format("{0}api/getObject.sjs?object=supporter&key={1}", salsaUrl, key);
                ;
                string result = ExtentedWebClient.Try(() => client.DownloadString(url), 3);
                Logger.Debug("response: " + result);
                XDocument xml = XDocument.Parse(result);
                return xml.Element("data").Element("supporter").Element("item");
            }
        }

        public void SaveSupporters(IEnumerable<NameValueCollection> supporters)
        {
            IEnumerable<Task> tasks = supporters.Select(supporter =>
                                                        Task.Factory.StartNew(wk =>
                                                                                  {
                                                                                      var id = SaveSupporter(supporter);
                                                                                      supporter["supporter_KEY"] = id;
                                                                                  }, null));
            Task.WaitAll(tasks.ToArray());
        }

        public void DeleteObjects(string objectType, IEnumerable<string> keys)
        {
            IEnumerable<Task> tasks = keys.Select(supporterKey =>
                                                  Task.Factory.StartNew(wk => DeleteObject(objectType, supporterKey),
                                                                        null));
            Task.WaitAll(tasks.ToArray());
        }

        public void DeleteAllObjects(string objectType)
        {
            SalsaGetObjects(objectType,
                            500,
                            supporters => DeleteObjects(objectType, supporters.Select(s => s.Element("key").Value)),
                            new List<string> {objectType + "_KEY"});
        }

        public int CustomColumnCount()
        {
            return CountObjects("custom_column");
        }

        private int CountObjects(string objectType)
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Counting Objects...");
                string url = salsaUrl + string.Format("api/getCount.sjs?object={0}&countColumn={0}_KEY", objectType);
                string result = ExtentedWebClient.Try(() => client.DownloadString(url), 3);
                Logger.Debug("response: " + result);
                XDocument xml = XDocument.Parse(result);
                string value = xml.Descendants("count").First().Value;
                return Int32.Parse(value);
            }
        }

        private string SalsaPost(string action, string objectType, NameValueCollection data)
        {
            string response;
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug(string.Format("POST to {0} {1} with {2}", action, objectType, data));
                data.Add("xml", "");
                data.Add("object", objectType);

                string url = salsaUrl + action;
                byte[] result = ExtentedWebClient.Try(() => client.UploadValues(url, "POST", data), 3);
                response = Encoding.UTF8.GetString(result);
                Logger.Debug("response: " + response);
            }
            return response;
        }
    }
}