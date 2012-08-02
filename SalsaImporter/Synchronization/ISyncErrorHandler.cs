using System;
using System.Collections.Specialized;
using SalsaImporter.Aft;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandlePullObjectFailure(ISyncObject obj, Exception ex);
        void HandleDeleteObjectFailure(string supporterKey);
        void HandlePushObjectFailure(ISyncObject obj, Exception ex);
    }
}