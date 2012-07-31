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
            var response = Try<HttpWebResponse, WebException>(() =>
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
            return Try <List<XElement>, InvalidSalsaResponseException>(() =>
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

        public void CreateSupporters(IEnumerable<NameValueCollection> supporters, Action<NameValueCollection> errorHandler = null)
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
                                                            catch(Exception)
                                                            {
                                                                if (errorHandler != null)
                                                                    errorHandler(nameValues);
                                                            }
                                                        }, supporterNameValues));
                Task.WaitAll(tasks.ToArray()); 
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
            return Try<string, InvalidSalsaResponseException>(() =>
                    {
                        string response = Post("save", objectType, data);
                        string supporterKeyFromServerResponse = GetObjectKeyFromServerResponse(response, data);
                        return supporterKeyFromServerResponse;
                    }, 3);
          
        }

        private static string GetObjectKeyFromServerResponse(string response, NameValueCollection data)
        {
            string key = null;
            try
            {
                var document = XDocument.Parse(response);
                key = document.Element("data").Element("success").Attribute("key").Value;
            }
            catch(Exception ex)
            {
                throw new InvalidSalsaResponseException(string.Format("Failed to get object key from server response:{0}.", response), ex);
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
            return Try<string, WebException>(() =>
                                             {
                                                 string response;
                                                 using (var client1 = new ExtentedWebClient(cookieContainer))
                                                 {
                                                     Logger.Debug(string.Format("POST to {0} {1} with {2}",
                                                                                action, objectType, data));
                                                     data.Set("xml", "");
                                                     data.Set("object", objectType);

                                                     string url = salsaUrl + action;
                                                     Logger.Trace("post:" + url);
                                                     byte[] result = client1.UploadValues(url, "POST", data);
                                                     response = Encoding.UTF8.GetString(result);
                                                     Logger.Trace("response from post: " + response);
                                                 }
                                                 return response;
                                             }, 3);
        }

        private string Get(string url)
        {
            Logger.Trace("Geting: " + url);
            CookieContainer cookieContainer = Login();
            string response = Try<string, WebException>(() =>
            {
                using (var webClient = new ExtentedWebClient(cookieContainer))
                {
                    string result = webClient.DownloadString(url);
                    return result;
                }
            }, 3);
            Logger.Trace("response from Get: " + response);
            return response;
        }

        public  TResult Try<TResult, TException>(Func<TResult> func, int tryTimes) where TException : Exception
        {
            int count = 0;

            while (true)
            {
                try
                {
                    return func();
                }
                catch (TException exception)
                {
                    Logger.Warn("SalsaClient catched exception and try again. Error:" + exception.Message);
                    count += 1;
                    if (count > tryTimes)
                    {
                        string message = String.Format("Rethrow exception after try {0} times. {1} ", tryTimes, exception.Message);
                        Logger.Error(message);
                        throw new ApplicationException(message);
                    }
                }
            }
        }

        

    }
}