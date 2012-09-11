using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Exceptions
{
    public class SaveToSalsaException : ApplicationException
    {
        public SaveToSalsaException(string message, Exception innerException = null): base(message, innerException){}
    }
}
