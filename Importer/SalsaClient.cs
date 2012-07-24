using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;

namespace Importer
{
    public class SalsaClient
    {
        readonly CookieContainer cookies = new CookieContainer();
        
        public void AuthenticateUser()
        {
            Logger.Debug("authenticating...");
            var url = ConfigurationManager.AppSettings["authenticationUrl"];
            var username = ConfigurationManager.AppSettings["salsaUserName"];
            var password = ConfigurationManager.AppSettings["salsaPassword"];
            var authenticate = WebRequest.Create(url + "api/authenticate.sjs?email=" + username + "&password=" + password);
            ((HttpWebRequest)authenticate).CookieContainer = cookies;
            authenticate.Method = "POST";
            authenticate.ContentType = "application/x-www-form-urlencoded";
            var authenticatedResponse = (HttpWebResponse)authenticate.GetResponse();
            Logger.Debug("response status code" + authenticatedResponse.StatusCode);
        }


        public Array PullObejcts()
        {
            Logger.Debug("Pulling objects...");
            var webRequest = new ExtentedWebClient(cookies, 30000);
            var url = ConfigurationManager.AppSettings["authenticationUrl"] + "api/getObjects.sjs?object=supporter";
            var response = webRequest.DownloadString(url);
            Logger.Debug("response from PulObjects: " + response);
            return null;
        }
        
        public void PushObject(NameValueCollection nameValues)
        {
            using (var client = new ExtentedWebClient(cookies, 3000))
            {
                Logger.Debug("Pushing Objects...");
                
                var url = ConfigurationManager.AppSettings["authenticationUrl"] + "save";
                byte[] result = client.UploadValues(url, "POST", nameValues);
                var response = Encoding.UTF8.GetString(result);
                Logger.Debug("response: " + response);
            }

        }
    }
}
