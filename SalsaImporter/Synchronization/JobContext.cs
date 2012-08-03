using System;
using SalsaImporter.Aft;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class JobContext: IJobContext
    {
        public event EventHandler JobContextChanged = delegate{};
        public int Id { get; set; }
        public string JobName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }

        public DateTime MinimumModificationDate { get { return SessionContext.MinimumModifiedDate; } }
        public int CurrentRecord { get; private set; }
        
        public void SetCurrentRecord(int newValue)
        {
            CurrentRecord = newValue;
            JobContextChanged(this, null);
        }

        public virtual SessionContext SessionContext { get; set; }

        public bool IsJobStarted
        {
            get { return StartTime != null; }
        }

        public bool IsJobFinished
        {
            get { return FinishedTime != null; }
        }
    }
}