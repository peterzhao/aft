using System;
using System.Collections.Specialized;
using SalsaImporter.Aft;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandlePushObjectFailure(NameValueCollection data);
        void HandlePullObjectFailure(ISyncObject obj, Exception ex);
        void HandleDeleteObjectFailure(string suppoertKey);
        void HandlePushObjectFailure(ISyncObject obj, Exception ex);
    }
}