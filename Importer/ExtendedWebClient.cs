using System;
using System.Net;

namespace Importer
{
    public class ExtentedWebClient : WebClient
    {
        private readonly CookieContainer _cookieContainer;
        private readonly int _timeout;

        public ExtentedWebClient(CookieContainer cookieContainer, int timeout)
        {

            this._cookieContainer = cookieContainer;
            _timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);

            var webRequest = request as HttpWebRequest;

            if (webRequest != null)
            {
                webRequest.CookieContainer = _cookieContainer;
                webRequest.Timeout = _timeout;
            }
            return request;
        }
    }
}