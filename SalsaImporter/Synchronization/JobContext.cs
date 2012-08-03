using System;
using SalsaImporter.Aft;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class JobContext: IJobContext
    {
      

        public int Id { get; set; }
        public int CurrentRecord { get; set; }
        public string JobName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }

        public DateTime MinimumModificationDate { get { return SessionContext.MinimumModifiedDate; } }

        public virtual SessionContext SessionContext { get; set; }

        public void MarkComplete()
        {
            throw new NotImplementedException();
        }

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