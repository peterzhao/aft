using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncLog
    {
        DateTime LastPushDateTime { get; set; }
        DateTime LastPullDateTime { get; set; }
        int LastPushedKey { get; set; }
        int LastPulledKey { get; set; }
        void PullingCompleted();
    }
}