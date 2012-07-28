using System;
using System.Collections.Specialized;
using System.Net;

namespace SalsaImporter
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


        public static TResult Try<TResult>(Func<TResult> func, int tryTimes)
        {
            int count = 0;

            while (count < tryTimes)
            {
                try
                {
                    return func();
                }
                catch (WebException exception)
                {
//                    if(exception.Message != "The operation has timed out")
//                        throw exception;
                    Logger.Warn("ExtendedWebClient catched WebException and try again.");
                }
                count += 1;
            }
            throw new WebException("Rethrow WebException after try " + tryTimes + " times.");
        }
    }
}