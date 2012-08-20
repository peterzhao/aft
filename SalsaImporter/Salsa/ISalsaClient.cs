using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Xml.Linq;

namespace SalsaImporter.Salsa
{
    public interface ISalsaClient
    {
        string Create(string objectType, NameValueCollection data);
        void Update(string objectType, NameValueCollection data, string[] fields = null);
        XElement GetObject(string objectType, string key);
        CookieContainer Login();

        List<XElement> GetObjects(string objectType, int batchSize, string startKey, DateTime lastPulledDate,
                                  IEnumerable<String> fieldsToReturn = null);

        DateTime CurrentTime { get; }
        void DeleteAllObjects(string objectType, int batchSize, bool fetchOnlyKeys);
        int CountObjects(string objectType);
    }
}