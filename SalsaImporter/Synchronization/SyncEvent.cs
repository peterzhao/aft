using System;
using System.ComponentModel.DataAnnotations;

namespace SalsaImporter.Synchronization
{
    public class SyncEvent
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int ExternalId { get; set; }
        public string ObjectType { get; set; }
        public string Data { get; set; }
        public string EventType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime TimeStamp { get; set; }

        public string Error { get; set; }
        public string Destination { get; set; }
        public virtual SessionContext SessionContext { get; set; }
    }
}