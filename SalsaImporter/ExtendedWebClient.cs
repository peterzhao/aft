using System;
using System.Net;
using System.Threading;

namespace SalsaImporter
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
                    Thread.Sleep(20000 * count); //wait for a while;
                    if (count > tryTimes)
                        throw new ApplicationException(String.Format(
                            "Rethrow WebException after try {0} times. {1} {2}", tryTimes, exception.Message,
                            exception.StackTrace));
                }
            }
        }
    }
}