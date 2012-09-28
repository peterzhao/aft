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
        private readonly FieldMapping _salsaKeyMapping = new FieldMapping {AftField = SyncObject.SalsaKeyColumnName, SalsaField = "key", DataType = "int"};
        public Mapper(string objectType, List<FieldMapping>  mappings)
        {
            _objectType = objectType;
            _mappings = mappings;
            _mappings.ForEach(m => _map.Add(m.AftField, m));
        }

        public FieldMapping PrimaryKeyMapping
        {
            get
            {
                var primaryMapping = _mappings.FirstOrDefault(m => m.MappingRule.EqualsIgnoreCase(MappingRules.primaryKey));
                return primaryMapping ?? _salsaKeyMapping;
            }
        }

        public NameValueCollection ToSalsa( SyncObject aftObject, SyncObject salsaObject)
        {
            var result = new NameValueCollection();
            _mappings.ForEach(fieldMapping =>
            {
                if (!aftObject.FieldNames.Contains(fieldMapping.AftField)) return;
                if (fieldMapping.MappingRule.EqualsIgnoreCase(MappingRules.readOnly)) return;
                if (fieldMapping.MappingRule.EqualsIgnoreCase(MappingRules.onlyIfBlank) && (salsaObject != null && !IsSalsaFieldBlank(salsaObject, fieldMapping))) return;
                if (fieldMapping.MappingRule.EqualsIgnoreCase(MappingRules.salsaWins) && salsaObject != null) return;
                SetFieldValueToSalsa(aftObject, fieldMapping, result);
            });
          
            SetPrimaryKeyToSalsa(aftObject, result);
            return result;
        }

       

        public bool IsIdentical(SyncObject aftObject, SyncObject salsaObject)
        {
            if (salsaObject == null) return false;
            foreach (var mapping in _mappings)
            {
                if (mapping.MappingRule.EqualsIgnoreCase(MappingRules.aftWins))
                {
                    if (IsObjectDifferent(aftObject, salsaObject, mapping)) return false;
                }
                if (mapping.MappingRule.EqualsIgnoreCase(MappingRules.onlyIfBlank) && IsSalsaFieldBlank(salsaObject, mapping))
                {
                    if (IsObjectDifferent(aftObject, salsaObject, mapping)) return false;
                }

            }
        
            return true;
        }


        private static bool IsSalsaFieldBlank(SyncObject salsaObject, FieldMapping fieldMapping)
        {
            if (fieldMapping.DataType.EqualsIgnoreCase(DataType.Boolean))
                return false; //blank cannot be applied to bool in salsa;
            return salsaObject[fieldMapping.AftField] == null;
        }

        private static bool IsObjectDifferent(SyncObject aftObject, SyncObject salsaObject, FieldMapping mapping)
        {
            var aftValue = aftObject[mapping.AftField];
            var salsValue = salsaObject[mapping.AftField];
            if (aftValue == null && salsValue == null) return false;
            if (aftValue == null) return true;
            return !aftValue.Equals(salsValue);
        }

        private  void SetPrimaryKeyToSalsa(SyncObject aftObject, NameValueCollection result)
        {
            SetFieldValueToSalsa(aftObject, PrimaryKeyMapping, result);
        }

        private  void SetFieldValueToSalsa(SyncObject aftObject, FieldMapping fieldMapping, NameValueCollection result)
        {
            var converter = DataTypeConverter.GetConverter(fieldMapping.DataType);
            result[fieldMapping.SalsaField] = converter.MakeSalsaValue(aftObject[fieldMapping.AftField]);
        }


        public SyncObject ToAft(XElement element)
        {
            if (!element.HasElements) return null;
            var syncObject = new SyncObject(_objectType);
            foreach (var mapping in _mappings)
            {
                if(mapping.MappingRule.EqualsIgnoreCase(MappingRules.writeOnly)) continue;
                var converter = DataTypeConverter.GetConverter(mapping.DataType);
                var value = element.StringValueOrNull(mapping.SalsaField);
                if (value == null) continue;
                syncObject[mapping.AftField] = converter.ReadSalsaValue(mapping.SalsaField, element);
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