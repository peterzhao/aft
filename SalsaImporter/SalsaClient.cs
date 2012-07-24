﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace SalsaImporter
{
    public class SalsaClient
    {
        private readonly CookieContainer cookies = new CookieContainer();

        public SalsaClient()
        {
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
        }

        public void Authenticate()
        {
            Logger.Debug("authenticating...");
            string url = ConfigurationManager.AppSettings["salsaApiUrl"];
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


        public string PullObejcts()
        {
            Logger.Debug("Pulling objects...");
            var webRequest = new ExtentedWebClient(cookies, 30000);
            string url = ConfigurationManager.AppSettings["salsaApiUrl"] + "api/getObjects.sjs?object=supporter";
            string response = webRequest.DownloadString(url);
            Logger.Debug("response from PulObjects: " + response);
            return response;
        }

        public void PullSupporters(Action<List<XElement>> batchHandler)
        {
            int start = 0;
            while(true)
            {
                Logger.Debug("Pulling objects... start: " + start);
                var webRequest = new ExtentedWebClient(cookies, 30000);
                string url =  String.Format("{0}api/getObjects.sjs?object=supporter&condition=supporter_KEY>{1}&limit=500&orderBy=supporter_KEY",
                    ConfigurationManager.AppSettings["salsaApiUrl"], start);
                string response = webRequest.DownloadString(url);
                //Logger.Debug("response from PulObjects: " + response);
                var supporters = XDocument.Parse(response).Descendants("item").ToList();
                start = int.Parse(supporters.Last().Element("supporter_KEY").Value);
                batchHandler(supporters);
                if (supporters.Count < 500) return;
                if (start == 0) throw new ApplicationException("Wrong response from server");
                
            }
        }

        public void PushObject(NameValueCollection nameValues)
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Pushing Objects...");

                string url = ConfigurationManager.AppSettings["salsaApiUrl"] + "save";
                byte[] result = client.UploadValues(url, "POST", nameValues);
                string response = Encoding.UTF8.GetString(result);
                Logger.Debug("response: " + response);
            }
        }

        public int Count()
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Counting Objects...");

                string url = ConfigurationManager.AppSettings["salsaApiUrl"] +
                             "api/getCount.sjs?object=supporter&countColumn=supporter_KEY";
                string result = client.DownloadString(url);
                Logger.Debug("response: " + result);
                XDocument xml = XDocument.Parse(result);
                string value = xml.Descendants("count").First().Value;
                return int.Parse(value);
            }
        }
    }
}