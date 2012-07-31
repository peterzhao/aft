using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncLog
    {
        DateTime LastPushDateTime { get; set; }
        DateTime LastPullDateTime { get; set; }
        DateTime CurrentDateTime { get; }
        int LastPushedKey { get; set; }
        int LastPulledKey { get; set; }
    }
}