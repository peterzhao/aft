using System;

namespace SalsaImporter.Synchronization
{
    public interface IJobContext
    {
        DateTime MinimumModificationDate { get; }
        int CurrentRecord { get; }
        void SetCurrentRecord(int newValue);
    }
}