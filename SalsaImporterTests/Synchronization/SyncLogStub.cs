using System;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    public class SyncLogStub: ISyncLog
    {
        public DateTime LastPushDateTime { get; set; }
        public DateTime LastPullDateTime { get; set; }
        public int LastPushedKey { get; set; }
        public int LastPulledKey { get; set; }

        public bool PullingCompletedCalled;

        public void PullingCompleted()
        {
            PullingCompletedCalled = true;
        }
    }
}