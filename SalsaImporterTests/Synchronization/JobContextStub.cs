using System;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    public class JobContextStub: IJobContext
    {
        public DateTime MinimumModificationDate { get; set; }
        public int CurrentRecord { get; private set; }
        public int? SuccessCount { get; set; }
        public int? ErrorCount { get; set; }

        public void SetCurrentRecord(int newValue)
        {
            CurrentRecord = newValue;
        }

        public void CountSuccess()
        {
            if (SuccessCount == null) SuccessCount = 0;
            SuccessCount = SuccessCount + 1;
        }

        public void CountError()
        {
            if (ErrorCount == null) ErrorCount = 0;
            ErrorCount = ErrorCount + 1;
        }
    }
}