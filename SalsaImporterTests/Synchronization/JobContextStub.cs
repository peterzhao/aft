using System;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    public class JobContextStub: IJobContext
    {
        public DateTime MinimumModificationDate { get; set; }
        public int CurrentRecord { get; private set; }
        public void SetCurrentRecord(int newValue)
        {
            CurrentRecord = newValue;
        }
    }
}