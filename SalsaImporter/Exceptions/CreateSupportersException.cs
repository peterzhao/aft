using System;

namespace SalsaImporter.Exceptions
{
    public class SyncAbortedException: ApplicationException
    {
        public SyncAbortedException(string message, Exception internalException = null)
            :base(message, internalException){}
    }
}
