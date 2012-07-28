using System;
using System.Net;

namespace SalsaImporter
{
    public class ExtentedWebClient : WebClient
    {
        private readonly CookieContainer _cookieContainer;
        private readonly int _timeout;

        public ExtentedWebClient(CookieContainer cookieContainer, int timeout)
        {
            _cookieContainer = cookieContainer;
            _timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

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

            while (true)
            {
                try
                {
                    return func();
                }
                catch (WebException exception)
                {
                    Logger.Warn("ExtendedWebClient catched WebException and try again. Error:" + exception.Message);
                    count += 1;
                    if (count > tryTimes)
                        throw new ApplicationException(String.Format(
                            "Rethrow WebException after try {0} times. {1} {2}", tryTimes, exception.Message,
                            exception.StackTrace));
                }
            }
        }
    }
}