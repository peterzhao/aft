using System;
using System.Xml.Linq;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class StringConverter : DataTypeConverter
    {
        public StringConverter(string datatype) : base(datatype)
        {
        }

        public override object ReadSalsaValue(string field, XElement element)
        {
            return element.StringValueOrNull(field);
        }

        public override string MakeSalsaValue(object value)
        {
            return string.IsNullOrWhiteSpace(value as string) ? " " : value.ToString();
        }

        public override object ReadAftValue(object value)
        {
            if (value.Equals(DBNull.Value)) return null;
            var str = value as string;
            return string.IsNullOrWhiteSpace(str) ? null : str.Trim(); //convert blank to null keeping the same login reading from salsa
        }
    }
}