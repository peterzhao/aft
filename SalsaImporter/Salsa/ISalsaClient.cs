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
        XElement GetObjectBy(string objectType, string salsaField, string value, IEnumerable<string> fieldsToReturn = null);
        CookieContainer Login();

        List<XElement> GetObjects(string objectType, int batchSize, int startKey, DateTime lastPulledDate, IEnumerable<string> fieldsToReturn = null);

        DateTime CurrentTime { get; }
        void DeleteAllObjects(string objectType, int batchSize, bool fetchOnlyKeys);
        int CountObjects(string objectType);
        int GetNextKey(string objectType, int originalStartKey, DateTime dateTime);
        List<string> GetFieldList(string objectType);
    }
}