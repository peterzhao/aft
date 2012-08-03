using System;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    public class SyncStateStub: ISyncState
    {
        public DateTime LastPushDateTime { get; set; }
        public DateTime MinimumModificationDate { get; set; }
        public int LastPushedKey { get; set; }
        public int CurrentRecord { get; set; }

        public bool PushingCompletedCalled;

        public bool PullingCompletedCalled;

        public void MarkComplete()
        {
            PullingCompletedCalled = true;
        }

        public void PushingCompleted()
        {
            PushingCompletedCalled = true;
        }
    }
}