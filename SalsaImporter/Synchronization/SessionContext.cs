using System;
using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public class SessionContext
    {
        public int Id { get; set; }
        public string State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime MinimumModifiedDate { get; set; }
        public virtual ICollection<JobContext> JobContexts { get; set; }
    }
}