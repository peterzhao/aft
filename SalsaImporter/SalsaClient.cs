﻿using System;
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
            salsaUrl = ConfigurationManager.AppSettings["salsaApiUrl"];
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
        }

        public void Authenticate()
        {
            Logger.Debug("authenticating...");
            string url = salsaUrl;
            string username = ConfigurationManager.AppSettings["salsaUserName"];
            string password = ConfigurationManager.AppSettings["salsaPassword"];
            WebRequest authenticate =
                WebRequest.Create(url + "api/authenticate.sjs?email=" + username + "&password=" + password);
            ((HttpWebRequest) authenticate).CookieContainer = cookies;
            authenticate.Method = "POST";
            authenticate.ContentType = "application/x-www-form-urlencoded";
            var authenticatedResponse = (HttpWebResponse) authenticate.GetResponse();
            Logger.Debug("response status code" + authenticatedResponse.StatusCode);
        }


        public void SalsaGetObjects(string objectType, int blockSize, Action<List<XElement>> batchHandler)
        {
            string keyFieldName = string.Format("{0}_KEY", objectType);
                
            int start = 0;
            while (true)
            {
                Logger.Debug("Pulling objects... start: " + start);
                var webRequest = new ExtentedWebClient(cookies, 30000);
                string url =
                    String.Format(
                        "{0}api/getObjects.sjs?object={1}&condition={2}>{3}&limit={4}&orderBy={2}",
                        salsaUrl, objectType, keyFieldName, start, blockSize);
                string response = webRequest.DownloadString(url);
                Logger.Debug("response from PullObjects: " + response);
                List<XElement> supporters = XDocument.Parse(response).Descendants("item").ToList();
                if (supporters.Count == 0) return;
                start = Int32.Parse(supporters.Last().Element(keyFieldName).Value);
                batchHandler(supporters);
                if (supporters.Count < blockSize) return;
                if (start == 0) throw new ApplicationException("Wrong response from server");
            }
        }

        public void DeleteSupporter(string supporterKey)
        {
            var key = new NameValueCollection
                                          {
                                              {"key", supporterKey}
                                          };
            SalsaPost("delete", "supporter", key);
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
                string result = client.DownloadString(url);
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

        public void DeleteSupporters(IEnumerable<string> keys)
        {
            IEnumerable<Task> tasks = keys.Select(supporterKey =>
                                                  Task.Factory.StartNew(wk => DeleteSupporter(supporterKey),
                                                                        null));
            Task.WaitAll(tasks.ToArray());
        }

        public void DeleteAllSupporters()
        {
            SalsaGetObjects("supporter", 500, supporters => DeleteSupporters(supporters.Select(s => s.Element("supporter_KEY").Value)));
        }

        public int CustomColumnCount()
        {
            return CountObjects("custom_column");
        }

//
//        public void DeleteAllCustomColumns()
//        {
//            SalsaGetObjects("custom_column", 500, supporters => DeleteSupporters(supporters.Select(s => s.Element("custom_column_KEY").Value)));
//        }
// 

        private int CountObjects(string objectType)
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Counting Objects...");
                string url = salsaUrl + string.Format("api/getCount.sjs?object={0}&countColumn={1}_KEY", objectType, objectType);
                string result = client.DownloadString(url);
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
                Logger.Debug("Pushing Objects...");
                data.Add("xml", "");
                data.Add("object", objectType);

                string url = salsaUrl + action;
                byte[] result = client.UploadValues(url, "POST", data);
                response = Encoding.UTF8.GetString(result);
                Logger.Debug("response: " + response);
            }
            return response;
        }

    }
}