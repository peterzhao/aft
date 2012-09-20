using System;

namespace SalsaImporter.Synchronization
{
    public interface IJobContext
    {
        DateTime MinimumModificationDate { get; }
        int CurrentRecord { get; }
        int? SuccessCount { get; set; }
        int? ErrorCount { get; set; }
        void SetCurrentRecord(int newValue);
        void CountSuccess();
        void CountError();
    }
}