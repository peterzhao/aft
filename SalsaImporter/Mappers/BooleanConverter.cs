using System;
using System.Xml.Linq;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class BooleanConverter: DataTypeConverter
    {
        public BooleanConverter(string datatype) : base(datatype)
        {
        }

        public override object ReadSalsaValue(string field, XElement element)
        {
            return element.BoolValueOrFalse(field);
        }

        public override string MakeSalsaValue(object value)
        {
            return value.Equals(true) ? "1" : "0";
        }

        public override object ReadAftValue(object value)
        {
            return value.Equals(DBNull.Value) ? false : value; //keep the same logic used to read bool from salsa
        }
    }
}