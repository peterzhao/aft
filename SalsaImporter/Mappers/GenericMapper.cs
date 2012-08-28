using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class GenericMapper: IMapper
    {
        private string _objectType;
        private Dictionary<string, string> _map;

        public GenericMapper(string objectType, Dictionary<string, string> map)
        {
            _objectType = objectType;
            _map = map;
        }

        protected Dictionary<string, string> Map { get { return _map; } }

        public NameValueCollection ToNameValues(SyncObject syncObject)
        {
            var result = new NameValueCollection();
            foreach (var property in syncObject.GetType().GetProperties())
            {
                var propertyName = property.Name;
                if (Map.ContainsKey(propertyName))
                {
                    object value = property.GetValue(syncObject, null);
                    if (value != null)
                    {
                        var stringValue = value.ToString();
                        if (property.PropertyType == typeof(bool)) stringValue = value.Equals(true) ? "1" : "0";
                        if (property.PropertyType == typeof(DateTime?)) stringValue = ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss");
                        result.Add(Map[propertyName], stringValue);
                    }
                }
            }
            return result;
        }

//        public SyncObject ToObject(XElement element)
//        {
//            var syncObject = new SyncObject(_objectType);
//            foreach (var property in syncObject.GetType().GetProperties())
//            {
//                string propertyName = property.Name;
//                var propertyType = property.PropertyType;
//                object propertyValue = null;
//                if (!Map.ContainsKey(propertyName)) continue;
//                if (propertyType == typeof(String))
//                    propertyValue = element.StringValueOrNull(Map[propertyName]);
//                else if (propertyType == typeof(int?))
//                    propertyValue = element.IntValueOrNull(Map[propertyName]);
//                else if (propertyType == typeof(int))
//                    propertyValue = element.IntValueOrDefault(Map[propertyName]);
//                else if (propertyType == typeof(float?))
//                    propertyValue = element.FloatValueOrNull(Map[propertyName]);
//                else if (propertyType == typeof(DateTime?))
//                    propertyValue = element.DateTimeValueOrNull(Map[propertyName]);
//                else if (propertyType == typeof(bool))
//                    propertyValue = element.BoolValueOrFalse(Map[propertyName]);
//                if (propertyValue != null)
//                    property.SetValue(syncObject, propertyValue, null);
//            }
//            return syncObject;
//        }

        public SyncObject ToObject(XElement element)
        {
            var syncObject = new SyncObject(_objectType);
            foreach (KeyValuePair<string, string> keyValuePair in Map)
            {
                string localName = keyValuePair.Key;
                string salsaName = keyValuePair.Value;
                syncObject[localName] = element.StringValueOrNull(salsaName);
            }
            return syncObject;
        }
    }
}