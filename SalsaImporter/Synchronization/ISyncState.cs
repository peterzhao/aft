using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncState
    {
        DateTime MinimumModificationDate { get; }
        int CurrentRecord { get; set; }
        void MarkComplete();
    }
}