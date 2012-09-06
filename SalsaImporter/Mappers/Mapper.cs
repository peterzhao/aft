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
        private readonly string _objectType;
        private readonly List<FieldMapping> _mappings;
        private readonly Dictionary<string, FieldMapping> _map = new Dictionary<string, FieldMapping>(); 

        public Mapper(string objectType, List<FieldMapping>  mappings)
        {
            _objectType = objectType;
            _mappings = mappings;
            _mappings.ForEach(m => _map.Add(m.AftField, m));
        }

        public NameValueCollection ToNameValues(SyncObject syncObject)
        {
            var result = new NameValueCollection();
            _mappings.Where(fieldMapping => !fieldMapping.MappingRule.EqualsIgnoreCase(MappingRules.readOnly)).ToList().ForEach(fieldMapping =>
                                  {
                                      if (!syncObject.FieldNames.Contains(fieldMapping.AftField)) return;
                                      var salsaField = fieldMapping.SalsaField;
                                      var fieldValue = syncObject[fieldMapping.AftField];
                                      var converter = DataTypeConverter.GetConverter(fieldMapping.DataType);

                                      result[salsaField] = converter.MakeSalsaValue(fieldValue);
                                  });
          
            result["key"] = syncObject.SalsaKey.ToString();
            return result;
        }

        public SyncObject ToObject(XElement element)
        {
            var syncObject = new SyncObject(_objectType);
            
            foreach (var mapping in _mappings)
            {
                string localName = mapping.AftField;
                string salsaName = mapping.SalsaField;
                var converter = DataTypeConverter.GetConverter(mapping.DataType);
                
                string value = element.StringValueOrNull(salsaName);
                if (value == null) continue;
                syncObject[localName] = converter.ReadSalsaValue(salsaName, element);
            }

            syncObject.SalsaKey = element.IntValueOrDefault("key");
            return syncObject;
        }

        public List<FieldMapping> Mappings { get { return _mappings; } }
    }

}



















//        public NameValueCollection ToNameValues(SyncObject syncObject)
//        {
//            var result = new NameValueCollection();
//            foreach (var property in syncObject.GetType().GetProperties())
//            {
//                var propertyName = property.Name;
//                if (Mappings.ContainsKey(propertyName))
//                {
//                    object value = property.GetValue(syncObject, null);
//                    if (value != null)
//                    {
//                        var stringValue = value.ToString();
//                        if (property.PropertyType == typeof(bool)) stringValue = value.Equals(true) ? "1" : "0";
//                        if (property.PropertyType == typeof(DateTime?)) stringValue = ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss");
//                        result.Add(Mappings[propertyName], stringValue);
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
//                if (!Mappings.ContainsKey(propertyName)) continue;
//                if (propertyType == typeof(String))
//                    propertyValue = element.StringValueOrNull(Mappings[propertyName]);
//                else if (propertyType == typeof(int?))
//                    propertyValue = element.IntValueOrNull(Mappings[propertyName]);
//                else if (propertyType == typeof(int))
//                    propertyValue = element.IntValueOrDefault(Mappings[propertyName]);
//                else if (propertyType == typeof(float?))
//                    propertyValue = element.FloatValueOrNull(Mappings[propertyName]);
//                else if (propertyType == typeof(DateTime?))
//                    propertyValue = element.DateTimeValueOrNull(Mappings[propertyName]);
//                else if (propertyType == typeof(bool))
//                    propertyValue = element.BoolValueOrFalse(Mappings[propertyName]);
//                if (propertyValue != null)
//                    property.SetValue(syncObject, propertyValue, null);
//            }
//            return syncObject;
//        }