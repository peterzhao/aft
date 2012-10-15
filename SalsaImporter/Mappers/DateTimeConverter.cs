using System;
using System.Xml.Linq;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class DateTimeConverter : DataTypeConverter
    {
        public DateTimeConverter(string datatype) : base(datatype)
        {
        }

        public override object ReadSalsaValue(string field, XElement element)
        {
            return element.DateTimeValueOrNull(field);
        }

        public override string MakeSalsaValue(object value)
        {
            if (value == null) return " ";
            if (value is DateTime)
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            return null;
        }

        public override object ReadAftValue(object value)
        {
            if (value.Equals(DBNull.Value)) return null;
            var date = (DateTime)value;
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
    }
}