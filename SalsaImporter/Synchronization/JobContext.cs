using System;

namespace SalsaImporter.Synchronization
{
    public class JobContext: IJobContext
    {
        public event EventHandler JobContextChanged = delegate{};
        public int Id { get; set; }
        public string JobName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public int? SuccessCount { get; set; }
        public int? ErrorCount { get; set; }

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

        public void CountSuccess()
        {
            if (SuccessCount == null) SuccessCount = 0;
            SuccessCount = SuccessCount + 1;
            JobContextChanged(this, null);
        }

        public void CountError()
        {
            if (ErrorCount == null) ErrorCount = 0;
            ErrorCount = ErrorCount + 1;
            JobContextChanged(this, null);
        }
    }
}