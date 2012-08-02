using System;
using System.ComponentModel.DataAnnotations;

namespace SalsaImporter.Aft
{
    public class SyncRun   
    {
        public int Id { get; set; }
        public bool Complete { get; set; }
        public DateTime StartTime { get; set; }
        public int CurrentRecord { get; set; }
        public DateTime LastUpdatedMinimum   { get; set; }
    }
}