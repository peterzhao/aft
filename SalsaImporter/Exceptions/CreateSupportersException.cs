using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Exceptions
{
    public class SyncCallendException: ApplicationException
    {
        public SyncCallendException(string message, Exception internalException = null)
            :base(message, internalException){}
    }
}
