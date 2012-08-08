using System;
using System.Net;

namespace SalsaImporter.Salsa
{
    public class ExtentedWebClient : WebClient
    {
        private readonly CookieContainer _cookieContainer;
        private const int Timeout = 30000;

        public ExtentedWebClient(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            var webRequest = request as HttpWebRequest;

            if (webRequest != null)
            {
                webRequest.CookieContainer = _cookieContainer;
                webRequest.Timeout = Timeout;
            }
            return request;
        }
    }
}