using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public abstract class GenericMapper<T> : IMapper where T: class, ISyncObject, new()
    {
        protected abstract Dictionary<string, string> Map { get; }

        public virtual string SalsaType { get {  return typeof(T).Name.ToLower(); } }


        public NameValueCollection ToNameValues(ISyncObject syncObject)
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

        public ISyncObject ToObject(XElement element)
        {
            var syncObject = new T();
            foreach (var property in syncObject.GetType().GetProperties())
            {
                string propertyName = property.Name;
                var propertyType = property.PropertyType;
                object propertyValue = null;
                if (!Map.ContainsKey(propertyName)) continue;
                if (propertyType == typeof(String))
                    propertyValue = element.StringValueOrNull(Map[propertyName]);
                else if (propertyType == typeof(int?))
                    propertyValue = element.IntValueOrNull(Map[propertyName]);
                else if (propertyType == typeof(int))
                    propertyValue = element.IntValueOrDefault(Map[propertyName]);
                else if (propertyType == typeof(float?))
                    propertyValue = element.FloatValueOrNull(Map[propertyName]);
                else if (propertyType == typeof(DateTime?))
                    propertyValue = element.DateTimeValueOrNull(Map[propertyName]);
                else if (propertyType == typeof(bool))
                    propertyValue = element.BoolValueOrFalse(Map[propertyName]);
                if (propertyValue != null)
                    property.SetValue(syncObject, propertyValue, null);
            }
            return syncObject;
        }

        public DynamicSyncObject ToDynamicObject(XElement element)
        {
            return new DynamicSyncObject();
        }
    }
}