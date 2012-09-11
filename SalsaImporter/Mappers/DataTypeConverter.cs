using System;
using System.Collections.Generic;
using System.Linq;
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
            new DataTypeConverter(DataType.Boolean, (field, element) => element.BoolValueOrFalse(field), BooleanToString),
            new DataTypeConverter(DataType.DateTime, (field, element) => element.DateTimeValueOrNull(field), DateTimeToString),
            new DataTypeConverter(DataType.Float, (field, element) => element.FloatValueOrNull(field)),
            new DataTypeConverter(DataType.Int, (field, element) => element.IntValueOrNull(field)),
            new DataTypeConverter(DataType.String, (field, element) => element.StringValueOrNull(field), value => value is string ? value as string : null)
        };

        private DataTypeConverter(string datatype, Func<string, XElement, object> readSalsaValue, Func<object, string> makeSalsaValue)
        {
            _datatype = datatype;
            _readSalsaValue = readSalsaValue;
            _makeSalsaValue = makeSalsaValue;
        }

        private DataTypeConverter(string datatype, Func<string, XElement, object> readSalsaValue)
            : this(datatype, readSalsaValue, value => DBNull.Value == value ? null : value == null ? null : value.ToString())
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
            if (value is DateTime)
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            return null;
        }
    }
}
