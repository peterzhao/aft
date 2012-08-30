using System;

namespace SalsaImporter.Exceptions
{
    public class InvalidDataTypeException : ApplicationException
    {
        public InvalidDataTypeException(string datatype) : base(string.Format("Unknown datatype: {0}", datatype))
        {
        }
    }
}
