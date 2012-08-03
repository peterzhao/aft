using System;

namespace SalsaImporter.Exceptions
{
    public class SyncCallendException: ApplicationException
    {
        public SyncCallendException(string message, Exception internalException = null)
            :base(message, internalException){}
    }
}
