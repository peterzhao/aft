using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
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
            new DataTypeConverter("boolean", (field, element) => element.BoolValueOrFalse(field), BooleanToString),
            new DataTypeConverter("datetime", (field, element) => element.DateTimeValueOrNull(field), DateTimeToString),
            new DataTypeConverter("float", (field, element) => element.FloatValueOrNull(field)),
            new DataTypeConverter("int", (field, element) => element.IntValueOrNull(field)),
            new DataTypeConverter("string", (field, element) => element.StringValueOrNull(field), value => (string)value)
        };

        private DataTypeConverter(string datatype, Func<string, XElement, object> readSalsaValue, Func<object, string> makeSalsaValue)
        {
            _datatype = datatype;
            _readSalsaValue = readSalsaValue;
            _makeSalsaValue = makeSalsaValue;
        }

        private DataTypeConverter(string datatype, Func<string, XElement, object> readSalsaValue)
            : this(datatype, readSalsaValue, value => value == null ? null : value.ToString())
        {
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
            var dataTypeConverter = DataTypeConverters.FirstOrDefault(converter => converter._datatype.EqualsIgnoreCase(datatype));
            if (dataTypeConverter == null) throw new InvalidDataTypeException(datatype);
            return dataTypeConverter;
        }

        private static string BooleanToString(object value)
        {
            return value.Equals(true) ? "1" : "0";
        }

        private static string DateTimeToString(object value)
        {
            return ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
