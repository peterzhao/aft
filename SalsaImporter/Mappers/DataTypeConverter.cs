using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class DataTypeConverter
    {
        private readonly string _datatype;
        private readonly Func<string, XElement, object> _readSalsaValue;
        private readonly Func<object, string> _makeSalsaValue;

        private static readonly List<DataTypeConverter> DataTypeConverters = new List<DataTypeConverter>
        {
            new DataTypeConverter("string", 
                                    (field, element) => element.StringValueOrNull(field),
                                    value => (string)value),
            new DataTypeConverter("datetime", 
                                   (field, element) => element.DateTimeValueOrNull(field),
                                   value => ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss")) 
        };

        private DataTypeConverter(string datatype, Func<string, XElement, object> readSalsaValue, Func<object, string> makeSalsaValue)
        {
            _datatype = datatype;
            _readSalsaValue = readSalsaValue;
            _makeSalsaValue = makeSalsaValue;
        }  

        public object ReadSalsaValue(string field, XElement element)
        {
            return _readSalsaValue(field, element);
        }

        public string MakeSalsaValue(object value)
        {
            return _makeSalsaValue(value);
        }

        public static DataTypeConverter GetConverter(string datatype)
        {
            return DataTypeConverters.First(converter => converter._datatype == datatype);
            
        }
    }
}
