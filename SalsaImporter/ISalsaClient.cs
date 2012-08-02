using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Xml.Linq;

namespace SalsaImporter
{
    public interface ISalsaClient
    {
        string Create(string objectType, NameValueCollection data);
        void Update(string objectType, NameValueCollection data, string[] fields = null);
        XElement GetObject(string key, string objectType);
        CookieContainer Login();
        List<XElement> GetObjects(string objectType, int batchSize, string startKey, DateTime lastPulledDate,
                                  IEnumerable<String> fieldsToReturn = null);
    }
}