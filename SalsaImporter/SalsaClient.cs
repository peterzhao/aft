using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Utilities;

namespace SalsaImporter
{
    public class SalsaClient
    {
        private readonly string salsaUrl;

        public SalsaClient()
        {
            salsaUrl = Config.SalsaApiUri;
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
        }

        public CookieContainer Login()
        {
            var cookieContainer = new CookieContainer();
            var response = ExtentedWebClient.Try(() =>
            {
                var webRequest =
                    WebRequest.Create(salsaUrl + "api/authenticate.sjs?email=" +
                                      Config.SalsaUserName + "&password=" + Config.SalsaPassword);
                ((HttpWebRequest)webRequest).CookieContainer = cookieContainer;
                webRequest.Method = "POST";
                webRequest.ContentLength = 0;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                return (HttpWebResponse)webRequest.GetResponse();
            }, 3);
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Logger.Debug("response: " + content);
            VerifyLoginResult(content);
            return cookieContainer;
        }

        private static void VerifyLoginResult(string content)
        {
            try
            {
                if (XDocument.Parse(content).Root.StringValueOrNull("message") != "Successful Login")
                    throw new ApplicationException("Login failed.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Login failed.", ex);
            }
        }


        public void EachBatchOfObjects(string objectType,
                                    int blockSize,
                                    Action<List<XElement>> batchHandler,
                                    IEnumerable<String> fieldsToReturn = null)
        {
            int start = 0;
            for (;;) // ever
            {
                string url = String.Format("{0}api/getObjects.sjs?object={1}&limit={2},{3}",salsaUrl, objectType, start, blockSize);

                if (fieldsToReturn != null)
                    url += "&include=" + String.Join(",", fieldsToReturn);

                string response = Get(url);
                                                

                List<XElement> supporters = XDocument.Parse(response).Descendants("item").ToList();

                if (supporters.Count == 0)break;

                batchHandler(supporters);
                start += blockSize;
                if (supporters.Count < blockSize) break;
            }
        }

        public void DeleteAllObjects(string objectType, int blockSize)
        {
            int start = 0;
            Logger.Info(String.Format("Deleting all {0}s...", objectType));
            for (; ; ) // ever
            {
                var items = GetBatchObjects(objectType, blockSize, start);
                if (items.Count == 0) break;
                DeleteObjects(objectType, items.Select(s => s.Element("key").Value));
                if (items.Count < blockSize) break;
            }
            Logger.Info(String.Format("All {0}s deleted.", objectType));
        }

        private List<XElement> GetBatchObjects(string objectType, int blockSize, int start)
        {
            string url = String.Format("{0}api/getObjects.sjs?object={1}&limit={2},{3}&include={1}_KEY",
                                       salsaUrl, objectType, start, blockSize);
            return Try(() =>
                    {
                        string response = Get(url);
                        List<XElement> items = ParseGetObjectResponseFromServer(response, objectType);
                        return items;
                    }, 3);
        }

        private  List<XElement> ParseGetObjectResponseFromServer(string response, string objectType)
        {
            try
            {
                XDocument responseXml = XDocument.Parse(response);
                List<XElement> items = responseXml.Element("data").Element(objectType).Elements("item").ToList();
                return items;
            }catch(Exception ex)
            {
                throw new InvalidSalsaResponseException(response, ex);
            }
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

        public void CreateSupporters(IEnumerable<NameValueCollection> supporters, Func<NameValueCollection, bool> errorHandler = null)
        {
            IEnumerable<Task> tasks = supporters.Select(supporterNameValues =>
                                                        Task.Factory.StartNew(arg =>
                                                        {
                                                            var nameValues = (NameValueCollection) arg;
                                                            try
                                                            {
                                                                var id = CreateSupporter(nameValues);
                                                                nameValues["supporter_KEY"] = id;
                                                            }
                                                            catch(Exception ex)
                                                            {
                                                                if(errorHandler == null || !errorHandler(nameValues))
                                                                    throw new ApplicationException("Create supporter failed", ex);
                                                            }
                                                        }, supporterNameValues));
            try
            {
                Task.WaitAll(tasks.ToArray(), -1); //no timeout
            }catch(AggregateException ex)
            {
                var message = "";
                ex.InnerExceptions.ToList().ForEach(e => message += ex.ToString() + "/n");
                throw new CreateSupportersException(string.Format("SalsaClient.CreateSupporters got {0} error(s): /n{1}", ex.InnerExceptions.Count, message), ex);
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

        public void CreateSupporterCustomColumn(NameValueCollection customField)
        {
            customField.Set("database_table_KEY", "142");
            customField.Set("key", "0"); // this is to indicate creation 
            customField.Set("required", "label,name,type"); 
            string response = Post("salsa/hq/addCustomColumn.jsp", "custom_column", customField);
        }

        private string Create(string objectType, NameValueCollection data)
        {
            data.Set("key", "0"); // this is to indicate creation  
            return Try(() =>
                    {
                        string response = Post("save", objectType, data);
                        string supporterKeyFromServerResponse = GetSupporterKeyFromServerResponse(response, data);
                        return supporterKeyFromServerResponse;
                    }, 3);
          
        }

        private static string GetSupporterKeyFromServerResponse(string response, NameValueCollection data)
        {
            string dateDetails = "Supporter data:";
            string key = null;
            data.AllKeys.ToList().ForEach(k => dateDetails += string.Format("{0}:{1} ", k, data[k]));
            try
            {
                var document = XDocument.Parse(response);
                key = document.Element("data").Element("success").Attribute("key").Value;
            }
            catch(Exception ex)
            {
                throw new InvalidSalsaResponseException(string.Format("Failed to get supporter key from server response:{0}. Supporter data:{1}", response, dateDetails), ex);
            }

            return key;
        }

        private int CountObjects(string objectType)
        {
            string result = Get(salsaUrl + string.Format("api/getCount.sjs?object={0}&countColumn={0}_KEY", objectType));
            string value = XDocument.Parse(result).Descendants("count").First().Value;
            return Int32.Parse(value);
        }

        private XElement GetObject(string key, string objectType)
        {
            string result = Get(String.Format("{0}api/getObject.sjs?object={1}&key={2}", salsaUrl, objectType, key));
            XDocument xml = XDocument.Parse(result);
            return xml.Element("data").Element(objectType).Element("item"); //Todo: check xml format for error.
        }

        private string Post(string action, string objectType, NameValueCollection data)
        {
            CookieContainer cookieContainer = Login();
            return ExtentedWebClient.Try(() =>
                                             {
                                                 string response1;
                                                 using (var client1 = new ExtentedWebClient(cookieContainer))
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

        private string Get(string url)
        {
            Logger.Debug("Requesting: " + url);
            CookieContainer cookieContainer = Login();
            string response = ExtentedWebClient.Try(() =>{
                using (var webClient = new ExtentedWebClient(cookieContainer))
                {
                    string result = webClient.DownloadString(url);
                    return result;
                }
                                                        }, 5);
            Logger.Debug("response from PullObjects: " + response);
            return response;
        }

        private  TResult Try<TResult>(Func<TResult> func, int tryTimes)
        {
            int count = 0;

            while (true)
            {
                try
                {
                    return func();
                }
                catch (InvalidSalsaResponseException exception)
                {
                    Logger.Warn("SalsaClient catched InvalidSalsaResponseException and try again. Error:" + exception.Message);
                    count += 1;
                    Thread.Sleep(5000 * count); //wait for a while;
                    if (count > tryTimes)
                        throw new ApplicationException(String.Format(
                            "Rethrow InvalidSalsaResponseException after try {0} times. {1} {2}", tryTimes, exception.Message,
                            exception.StackTrace));
                }
            }
        }

        

    }
}