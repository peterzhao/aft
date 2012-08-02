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
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter
{
    public class SalsaClient : ISalsaClient
    {
        private readonly string _salsaUrl;
        private SyncErrorHandler _errorHandler;

        public SalsaClient(SyncErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _salsaUrl = Config.SalsaApiUri;
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return (true); };
        }

        public string Create(string objectType, NameValueCollection data)
        {
            data.Set("key", "0"); // this is to indicate creation  
            return SyncErrorHandler.Try<string, InvalidSalsaResponseException>(() =>
            {
                string response = Post("save", objectType, data);
                return VerifySaveResponse(response, data);
            }, 3);

        }

        public void Update(string objectType, NameValueCollection data, string[] fields = null)
        {
            var subSetData = new NameValueCollection();
            data.AllKeys.ToList().ForEach(k =>
                                              {
                                                  if (fields == null || fields.Contains(k) || k == "key")
                                                      subSetData[k] = data[k];
                                              });
           
            SyncErrorHandler.Try<string, InvalidSalsaResponseException>(() =>
            {
                string response = Post("save", objectType, subSetData);
                VerifySaveResponse(response, data);
                return null;
            }, 3);

        }

        public XElement GetObject(string key, string objectType)
        {
            string result = Get(String.Format("{0}api/getObject.sjs?object={1}&key={2}", _salsaUrl, objectType, key));
            XDocument xml = XDocument.Parse(result);
            return xml.Element("data").Element(objectType).Element("item"); //Todo: check xml format for error.
        }





        public CookieContainer Login()
        {
            var cookieContainer = new CookieContainer();
            var response = SyncErrorHandler.Try<HttpWebResponse, WebException>(() =>
            {
                var webRequest =
                    WebRequest.Create(_salsaUrl + "api/authenticate.sjs?email=" +
                                      Config.SalsaUserName + "&password=" + Config.SalsaPassword);
                ((HttpWebRequest)webRequest).CookieContainer = cookieContainer;
                webRequest.Method = "POST";
                webRequest.ContentLength = 0;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                return (HttpWebResponse)webRequest.GetResponse();
            }, 3);
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Logger.Trace("response: " + content);
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
                string url = String.Format("{0}api/getObjects.sjs?object={1}&limit={2},{3}",_salsaUrl, objectType, start, blockSize);

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

        public void DeleteAllObjects(string objectType, int blockSize, bool fetchOnlyKeys)
        {
            int start = 0;
            Logger.Info(String.Format("Deleting all {0}s...", objectType));
            for (; ; ) // ever
            {
                var items = GetBatchObjects(objectType, blockSize, start, fetchOnlyKeys);
                if (items.Count == 0) break;
                DeleteObjects(objectType, items.Select(s => s.Element("key").Value));
                if (items.Count < blockSize) break;
            }
            Logger.Info(String.Format("All {0}s deleted.", objectType));
        }

        private List<XElement> GetBatchObjects(string objectType, int blockSize, int start, bool fetchOnlyKeys)
        {
            string url = String.Format("{0}api/getObjects.sjs?object={1}&limit={2},{3}",
                                       _salsaUrl, objectType, start, blockSize);
            
            if (fetchOnlyKeys)
            {
                url += "&include=" + objectType + "_KEY";
            }

            return SyncErrorHandler.Try<List<XElement>, InvalidSalsaResponseException>(() =>
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

        public void CreateSupporters(IEnumerable<NameValueCollection> supporters)
        {
            IEnumerable<Task> tasks = supporters.Select(supporterNameValues =>
                                                        Task.Factory.StartNew(arg =>
                                                        {
                                                            try
                                                            {
                                                                var id = CreateSupporter(supporterNameValues);
                                                                supporterNameValues["supporter_KEY"] = id;
                                                            }
                                                            catch(Exception)
                                                            {
                                                                _errorHandler.HandlePushObjectFailure(supporterNameValues);
                                                            }
                                                        }, null));
                Task.WaitAll(tasks.ToArray()); 
        }

        public void DeleteObjects(string objectType, IEnumerable<string> keys)
        {
            Logger.Debug("Deleting objects");
            IEnumerable<Task> tasks = keys.Select(key =>
                                                  Task.Factory.StartNew(arg =>
                                                  {
                                                      try
                                                      {
                                                          DeleteObject(objectType, key);
                                                      }
                                                      catch(Exception)
                                                      {
                                                          _errorHandler.HandleDeleteObjectFailure(key);
                                                      }
                                                  }, null));
          
            Task.WaitAll(tasks.ToArray(), -1); // no timeout
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

      

        private static string VerifySaveResponse(string response, NameValueCollection data)
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
            string result = Get(_salsaUrl + string.Format("api/getCount.sjs?object={0}&countColumn={0}_KEY", objectType));
            string value = XDocument.Parse(result).Descendants("count").First().Value;
            return Int32.Parse(value);
        }

      

        private string Post(string action, string objectType, NameValueCollection data)
        {
            CookieContainer cookieContainer = Login();
            return SyncErrorHandler.Try<string, WebException>(() =>
                                             {
                                                 string response;
                                                 using (var client1 = new ExtentedWebClient(cookieContainer))
                                                 {
                                                     Logger.Trace(string.Format("POST to {0} {1} with {2}",
                                                                                action, objectType, data));
                                                     data.Set("xml", "");
                                                     data.Set("object", objectType);

                                                     string url = _salsaUrl + action;
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
            string response = SyncErrorHandler.Try<string, WebException>(() =>
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

        public List<XElement> GetObjects(string objectType, int batchSize, string startKey, DateTime lastPulledDate, IEnumerable<String> fieldsToReturn = null)
        {
            var formats = "yyyy-MM-dd HH:mm:ss";
            var url = String.Format("{0}api/getObjects.sjs?object={1}&condition={1}_KEY>{2}&condition=Last_Modified>{4}&limit={3}&orderBy={1}_KEY",
                                       _salsaUrl, objectType, startKey, batchSize, lastPulledDate.ToString(formats));

            if (fieldsToReturn != null)
                url += "&include=" + String.Join(",", fieldsToReturn);

            return SyncErrorHandler.Try<List<XElement>, InvalidSalsaResponseException>(() =>
                                                                                           {
                                                                                               var response = Get(url);
                                                                                               VerifyGetObjectsResponse(response, objectType);
                                                                                               return XDocument.Parse(response).Descendants("item").ToList();
                                                                                           }, 3);

        }

        private void VerifyGetObjectsResponse(string response, string objectType)
        {
            try
            {
                var document = XDocument.Parse(response);
                document.Element("data").Element(objectType);
            }
            catch (Exception ex)
            {
                throw new InvalidSalsaResponseException(string.Format("Get invalid response from server when get objects:{0}.", response), ex);
            }
        }
    }
}