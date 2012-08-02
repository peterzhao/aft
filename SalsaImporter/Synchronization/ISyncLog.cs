using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncLog
    {
        DateTime LastPullDateTime { get; }
        int LastPulledKey { get; set; }
        void PullingCompleted();
        void PushingCompleted();

        DateTime LastPushDateTime { get; set; }
        int LastPushedKey { get; set; }
    }
}