using System;

namespace SalsaImporter.Exceptions
{
    public class InvalidSalsaResponseException : ApplicationException
    {
        public InvalidSalsaResponseException(string message, Exception internalException)
            : base(message, internalException)
        {
        }
    }
}