using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Exceptions
{
    public class CreateSupportersException: ApplicationException
    {
        public CreateSupportersException(string message, Exception internalException)
            :base(message, internalException){}
    }
}
