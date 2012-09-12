using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Exceptions
{
    public class SalsaClientException : ApplicationException
    {
        public SalsaClientException(string message, Exception innerException = null) : base(message, innerException) { }
    }
}
