using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;

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

        public void AuthenticateUser()
        {
            Logger.Debug("authenticating...");
            string url = ConfigurationManager.AppSettings["authenticationUrl"];
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
            string url = ConfigurationManager.AppSettings["authenticationUrl"] + "api/getObjects.sjs?object=supporter";
            string response = webRequest.DownloadString(url);
            Logger.Debug("response from PulObjects: " + response);
            return response;
        }

        public void PushObject(NameValueCollection nameValues)
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Pushing Objects...");

                string url = ConfigurationManager.AppSettings["authenticationUrl"] + "save";
                byte[] result = client.UploadValues(url, "POST", nameValues);
                string response = Encoding.UTF8.GetString(result);
                Logger.Debug("response: " + response);
            }
        }
    }
}