using System;
using System.Collections.Specialized;
using SalsaImporter.Aft;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandleAddObjectFailure(ISyncObject obj, Exception ex);
        void HandleDeleteObjectFailure(string supporterKey);
    }
}