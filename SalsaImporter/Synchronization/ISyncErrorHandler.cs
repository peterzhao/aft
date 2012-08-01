using System;
using System.Collections.Specialized;

namespace SalsaImporter.Synchronization
{
    public interface ISyncErrorHandler
    {
        void HandlePushObjectFailure(NameValueCollection data);
        void HandlePullObjectFailure(ISyncObject obj, Exception ex);
        void HandleDeleteObjectFailure(string suppoertKey);
    }
}