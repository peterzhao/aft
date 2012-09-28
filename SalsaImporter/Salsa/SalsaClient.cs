using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Salsa
{
    public class SalsaClient : ISalsaClient
    {
        private const string DataElementName = "data";
        private const string ItemElementName = "item";
        private const string CountElementName = "count";
        private const string ResultElementName = "result";

        private readonly string _salsaUrl;
        private readonly Boolean _writeAccessEnabled;
        private DateTime DateBeforeCreationOfAllObjects;

        public SalsaClient()
        {
            _salsaUrl = Config.SalsaApiUri;
            _writeAccessEnabled = Config.SalsaWritable;

            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return (true); };
            DateBeforeCreationOfAllObjects = new DateTime(1991, 1, 1);
        }

        public string Save(string objectType, NameValueCollection data)
        {
            return Post("save", objectType, data, response =>
                                                      {
                                                          string key = null;
                                                          try
                                                          {
                                                              var document = XDocument.Parse(response);
                                                              key = document.Element(DataElementName).Element("success").Attribute("key").Value;
                                                          }
                                                          catch (Exception ex)
                                                          {
                                                              throw new InvalidSalsaResponseException(String.Format("Failed to get object key from server response:{0}.", response), ex);
                                                          }

                                                          return key;
                                                      });
        }

        public XElement GetObject(string objectType, string key)
        {
            var url = String.Format("{0}api/getObject.sjs?object={1}&key={2}", _salsaUrl, objectType, key);
            return Get(url, (response) =>
                                {
                                    try
                                    {
                                        var xml = XDocument.Parse(response);
                                        return xml.Element(DataElementName).Element(objectType).Element(ItemElementName);
                                    }catch(Exception ex)
                                    {
                                        throw new InvalidSalsaResponseException(string.Format("Could not parse response to get object. {0}", response),ex);
                                    }
                                });
        }

        public XElement GetObjectBy(string objectType, string salsaField, string value, IEnumerable<string> fieldsToReturn = null)
        {
            if (salsaField == "key") return GetObject(objectType, value);
            var url = String.Format("{0}api/getObjects.sjs?object={1}&condition={2}={3}", _salsaUrl, objectType, salsaField, value);
            if (fieldsToReturn != null)
                url += "&include=" + String.Join(",", fieldsToReturn);
            return Get(url, (response) =>
                                {
                                    try
                                    {
                                        VerifyGetObjectsResponse(response, objectType);
                                        var xml = XDocument.Parse(response);
                                        var objectTopElement = xml.Element(DataElementName).Element(objectType);
                                        return objectTopElement.Element(CountElementName).Value == "0"
                                                   ? XElement.Parse("<item/>")
                                                   : objectTopElement.Element(ItemElementName);
                                     }catch(Exception ex)
                                    {
                                        throw new InvalidSalsaResponseException(string.Format("Could not parse response to get object. {0}", response),ex);
                                    }
                                });
          
        }
    

        public CookieContainer Login()
        {
            var cookieContainer = new CookieContainer();
            var response = Try<HttpWebResponse, WebException>(() =>
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
            string content = new StreamReader((Stream) response.GetResponseStream()).ReadToEnd();
            Logger.Trace("response: " + content);
            VerifyLoginResult(_salsaUrl, Config.SalsaUserName, content);
            return cookieContainer;
        }

        public void DeleteAllObjects(string objectType, int blockSize, bool fetchOnlyKeys)
        {
            int start = 0;
            Logger.Info(String.Format("Deleting all {0}s...", objectType));
            for (; ; ) // ever
            {
                List<string> fieldsToReturn = null;
                if (fetchOnlyKeys)
                    fieldsToReturn = ListOfOnlyPrimaryKeyFieldName(objectType);
 
                List<XElement> items = GetObjects(objectType, blockSize, start, DateBeforeCreationOfAllObjects, fieldsToReturn);
 
                if (items.Count == 0) break;
                DeleteObjects(objectType, items.Select(s => s.Element("key").Value));
                if (items.Count < blockSize) break;
            }
            Logger.Info(String.Format("All {0}s deleted.", objectType));
        }

        private static List<string> ListOfOnlyPrimaryKeyFieldName(string objectType)
        {
            return new List<string> { objectType + "_KEY" };
        }

        public void DeleteObject(string objectType, string key)
        {
            var data = new NameValueCollection { { "key", key } };
            Post("delete", objectType, data, response => "");
        }

        public void DeleteObjects(string objectType, IEnumerable<string> keys)
        {
            Logger.Debug(String.Format("Deleting {0} {1}(s) in {2}", keys.Count(), objectType, Config.Environment));
            IEnumerable<Task> tasks = keys.Select(key =>
                                                  Task.Factory.StartNew(arg => DeleteObject(objectType, key), null));

            Task.WaitAll(tasks.ToArray(), -1); // no timeout
        }

        public int CountObjects(string objectType)
        {
            return CountObjectsMatchingQuery(objectType, null, null, null);
        }

        public int GetNextKey(string objectType, int startKey, DateTime lastModifiedDate)
        {
            var xElements = GetObjects(objectType, 1, startKey, lastModifiedDate, ListOfOnlyPrimaryKeyFieldName(objectType));
            var firstElement = xElements.First();
            return int.Parse(firstElement.Element(objectType + "_KEY").Value);
        }

        public List<string> GetFieldList(string objectType)
        {
            var url = String.Format("{0}api/getObjects.sjs?object={1}&limit=1",_salsaUrl, objectType);

            return Get(url, response =>
            {
                VerifyGetObjectsResponse(response, objectType);
                return XDocument.Parse(response).Descendants(ItemElementName).First().Elements().Select(e => e.Name.LocalName).ToList();
            });
        }

        public bool DoesMembershipExist(string groupType, string objectType, string groupKey, string objectKey)
        {
            var condition1 = String.Format("condition={0}_KEY={1}", groupType, groupKey);
            var condition2 = String.Format("condition={0}_KEY={1}", objectType, objectKey);
            var queryString =  String.Format("api/getCount.sjs?object={0}_{1}&{2}&{3}&countColumn={0}_{1}_KEY",
                                 objectType, groupType, condition1, condition2);
                var url = _salsaUrl + queryString;
                return Get(url, (result) =>
                {
                    var value = XDocument.Parse(result).Descendants(CountElementName).First().Value;
                    var exist = Int32.Parse(value) > 0;
                    Logger.Trace(string.Format("Membership exist? {0} for {1}_{2} with {1}_key: {3} {2}_key: {4}", exist, objectType, groupType, groupKey, objectKey));
                    return exist;
                }); 
        }

        public int CountObjectsMatchingQuery(string objectType, string conditionName, string comparator, string conditionValue ) 
        {
            var queryString = GetQueryString(objectType, conditionName, comparator, conditionValue);
            var url = _salsaUrl + queryString;
            return Get(url, (result) =>
                                {
                                    var value = XDocument.Parse(result).Descendants(CountElementName).First().Value;
                                    return Int32.Parse(value);
                                });
            
        }

        public static string GetQueryString(string objectType, string conditionName, string comparator, string conditionValue)
        {
            if (String.IsNullOrEmpty(objectType))
            {
                throw new ArgumentException("objectType must be provided");
            }

            if (conditionName != null && conditionValue != null && comparator != null)
            {
                string condition = String.Format("{0}{1}{2}", conditionName, comparator, conditionValue);
                return String.Format("api/getCount.sjs?object={0}&condition={1}&countColumn={0}_KEY", objectType,
                                     condition);
            }
            return String.Format("api/getCount.sjs?object={0}&countColumn={0}_KEY", objectType);
        }

        public List<XElement> GetObjects(string objectType, int batchSize, int startKey, DateTime lastPulledDate, IEnumerable<string> fieldsToReturn = null)
        {
            var formats = "yyyy-MM-dd HH:mm:ss";

            var url = String.Format("{0}api/getObjects.sjs?object={1}&condition={1}_KEY>{2}&condition=Last_Modified>{4}&limit={3}&orderBy={1}_KEY",
                                       _salsaUrl, objectType, startKey, batchSize, lastPulledDate.ToString(formats));

            if (fieldsToReturn != null)
                url += "&include=" + String.Join(",", fieldsToReturn);

            return Get(url, response =>
                                {
                                    VerifyGetObjectsResponse(response, objectType);
                                    return XDocument.Parse(response).Descendants(ItemElementName).ToList();
                                });
          
        }

        public void CreateSupporterCustomColumn(NameValueCollection customField)
        {
            customField.Set("database_table_KEY", "142");
            customField.Set("key", "0"); // this is to indicate creation
            customField.Set("expose_to_chapters", "1");
            customField.Set("required", "label,name,type");
            Post("salsa/hq/addCustomColumn.jsp", "custom_column", customField, resonse => "");
        }

        public DateTime CurrentTime
        {
            get
            {
                const string currentTimeObjectType = "bad_word";
                const string currentTimeObjectTypeDateField = "Date_Created";

                var newId = Save(currentTimeObjectType, new NameValueCollection());
                var newObject = GetObject(currentTimeObjectType, newId);
                var currentTime = newObject.DateTimeValueOrNull(currentTimeObjectTypeDateField);
                DeleteObject(currentTimeObjectType, newId);
                return (DateTime)currentTime;
            }
        }

        private void VerifyLoginResult(string salsaUrl, string salsaUserName, string content)
        {
            try
            {
                if (XDocument.Parse(content).Root.StringValueOrNull("message") != "Successful Login")
                    throw new ApplicationException(string.Format("Login to {0} as {1} failed.", salsaUrl, salsaUserName));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Login to {0} as {1} failed.", salsaUrl, salsaUserName), ex);
            }
        }

        private T Post<T>(string action, string objectType, NameValueCollection data, Func<string, T> callBack )
        {
            EnsureWriteAccess();
            var url = _salsaUrl + action;
            data.Set("xml", "");
            data.Set("object", objectType);
            var dataString = String.Join("&", data.AllKeys.ToList().Select(key => key + "=" + data[key]));
            var info = "Post:" + url + " data:" + dataString;

            var cookieContainer = Login();
            
            return Try<T, Exception>(() =>
            {
                string response;
                using (var client = new ExtentedWebClient(cookieContainer))
                {
                    Logger.Trace(info);
                    var result = client.UploadValues(url, "POST", data);
                    response = Encoding.UTF8.GetString(result);
                    Logger.Trace("response from post: " + response);
                }
                return callBack(response);
            }, 3, info);
        }

        private T Get<T>(string url, Func<string, T> callBack)
        {
            var cookieContainer = Login();
            return Try<T, Exception>(() =>
                                         {
                                             Logger.Trace("Geting: " + url);
                                             string response = null;
                                             using (var webClient = new ExtentedWebClient(cookieContainer))
                                             {
                                                 response = webClient.DownloadString(url);
                                             }
          
                                             Logger.Trace("response from Get: " + response);
                                             return callBack(response);
                                         }, 3, string.Format("Try to get: {0}.",url));
        }

        private void VerifyGetObjectsResponse(string response, string objectType)
        {
           
                var document = XDocument.Parse(response);
                
                var objectTypeElement = document.Element(DataElementName).Element(objectType);
                if (objectTypeElement == null)
                {
                    throw new InvalidSalsaResponseException(String.Format("Response contains no object type element:\n{0}", response));   
                }

                var countElement = objectTypeElement.Element(CountElementName);
                if (countElement != null && countElement.Value == "0")
                    return;

                var resultElement = objectTypeElement.Element(ItemElementName).Element(ResultElementName);
                if (resultElement != null && resultElement.Value == "error")
                {
                    throw new InvalidSalsaResponseException(String.Format("Response from server indicates error:\n{0}", response));  
                }
           
        }

        private void EnsureWriteAccess()
        {
            if (!_writeAccessEnabled)
            {
                throw new AccessViolationException(String.Format("{0} is in read-only mode", Config.Environment));
            }
        }

        public static TResult Try<TResult, TException>(Func<TResult> func, int tryTimes, string info = null) where TException : Exception
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
                    string exceptionName = exception.GetType().Name;
                    count += 1;
                    if (count >= tryTimes)
                    {
                        string message = String.Format("Rethrow {0} after try {1} times. {2} Error: {3} ", exceptionName, tryTimes, info, exception.Message);
                        Logger.Error(message, exception);
                        throw new SalsaClientException(message);
                    }
                    else
                    {
                        string message = String.Format("Caught {0} on attempt {1}. Trying again. Error: {2}", exceptionName, count, exception.Message);
                        Logger.Warn(message, exception);
                    }
                }
            }
        }


    }
}