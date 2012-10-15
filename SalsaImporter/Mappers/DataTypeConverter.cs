using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public abstract class DataTypeConverter
    {
        protected readonly string Datatype;

        private static readonly List<DataTypeConverter> DataTypeConverters = new List<DataTypeConverter>
        {
            new BooleanConverter(DataType.Boolean),
            new DateTimeConverter(DataType.DateTime),
            new FloatConverter(DataType.Float),
            new IntConverter(DataType.Int),
            new StringConverter(DataType.String)
        };


        public static DataTypeConverter GetConverter(string datatype)
        {
            var dataTypeConverter = DataTypeConverters.FirstOrDefault(converter => converter.Datatype.EqualsIgnoreCase(datatype));
            if (dataTypeConverter == null) throw new InvalidDataTypeException(datatype);
            return dataTypeConverter;
        }

        protected DataTypeConverter(string datatype)
        {
            Datatype = datatype;
        }



        public abstract object ReadSalsaValue(string field, XElement element);
        public abstract string MakeSalsaValue(object value);
        public abstract object ReadAftValue(object value);

    }
}
