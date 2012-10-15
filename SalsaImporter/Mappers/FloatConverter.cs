using System;
using System.Xml.Linq;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class FloatConverter : DataTypeConverter
    {
        public FloatConverter(string datatype) : base(datatype)
        {
        }

        public override object ReadSalsaValue(string field, XElement element)
        {
            var value = element.FloatValueOrNull(field);
            return value == 0.00 || value == 0 ? null : value;
        }

        public override string MakeSalsaValue(object value)
        {
            return value == null ? "0.0" : value.ToString();
        }

        public override object ReadAftValue(object value)
        {
            if (value.Equals(DBNull.Value)) return null;
            if (value.Equals(0.0) || value.Equals(0)) return null;
            return value;
        }
    }
}