using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Xml.Linq;

namespace SalsaImporter.Salsa
{
    public interface ISalsaClient
    {
        string Save(string objectType, NameValueCollection data);
        XElement GetObject(string objectType, string key);
        XElement GetObjectBy(string objectType, string salsaField, string value);
        CookieContainer Login();

        List<XElement> GetObjects(string objectType, int batchSize, string startKey, DateTime lastPulledDate,
                                  IEnumerable<String> fieldsToReturn = null);

        DateTime CurrentTime { get; }
        void DeleteAllObjects(string objectType, int batchSize, bool fetchOnlyKeys);
        int CountObjects(string objectType);
    }
}