using System;

namespace SalsaImporter.Synchronization
{
    public class JobContext: IJobContext
    {
        private readonly Object _locker = new Object();
        public event EventHandler JobContextChanged = delegate{};
        public int Id { get; set; }
        public string JobName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public int? SuccessCount { get; set; }
        public int? ErrorCount { get; set; }

        public DateTime MinimumModificationDate { get { return SessionContext.MinimumModifiedDate; } }
        public int CurrentRecord { get; private set; }

        public int? IdenticalObjectCount { get; set; }

      
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
            lock (_locker)
            {
                if (SuccessCount == null) SuccessCount = 0;
                SuccessCount = SuccessCount + 1;
            }
           
        }

        public void CountError()
        {
            lock (_locker)
            {
                if (ErrorCount == null) ErrorCount = 0;
                ErrorCount = ErrorCount + 1;
            }
        }

        public void CountIdenticalObject()
        {
            lock (_locker)
            {
                if (IdenticalObjectCount == null) IdenticalObjectCount = 0;
                IdenticalObjectCount = IdenticalObjectCount + 1;  
            }
        }
    }
}