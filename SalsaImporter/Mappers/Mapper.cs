using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class Mapper: IMapper
    {
        private string _objectType;
        private List<FieldMapping> _mappings;
        private Dictionary<string, FieldMapping> _map = new Dictionary<string, FieldMapping>(); 

        public Mapper(string objectType, List<FieldMapping>  mappings)
        {
            _objectType = objectType;
            _mappings = mappings;
            _mappings.ForEach(m => _map.Add(m.AftField, m));
        }


        public NameValueCollection ToNameValues(SyncObject syncObject)
        {
            var result = new NameValueCollection();
            _map.Keys.ToList().ForEach(key=>result[_map[key].SalsaField] = syncObject[key]);
            //syncObject.FieldNames.ForEach(f => result[_map[f].SalsaField] = syncObject[f]);
            return result;
        }

//        public NameValueCollection ToNameValues(SyncObject syncObject)
//        {
//            var result = new NameValueCollection();
//            foreach (var property in syncObject.GetType().GetProperties())
//            {
//                var propertyName = property.Name;
//                if (Map.ContainsKey(propertyName))
//                {
//                    object value = property.GetValue(syncObject, null);
//                    if (value != null)
//                    {
//                        var stringValue = value.ToString();
//                        if (property.PropertyType == typeof(bool)) stringValue = value.Equals(true) ? "1" : "0";
//                        if (property.PropertyType == typeof(DateTime?)) stringValue = ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss");
//                        result.Add(Map[propertyName], stringValue);
//                    }
//                }
//            }
//            return result;
//        }

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
            foreach (var mapping in _mappings)
            {
                string localName = mapping.AftField;
                string salsaName = mapping.SalsaField;
                string value = element.StringValueOrNull(salsaName);
                if (value == null) continue;
                syncObject[localName] = value;
            }
            syncObject.Id = element.IntValueOrDefault("key");
            return syncObject;
        }
    }
}