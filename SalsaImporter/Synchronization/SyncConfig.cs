using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Synchronization
{
    public class SyncConfig
    {
        public int Id { get; set; }
        public string ObjectType { get; set; }
        public string SyncDirection { get; set; }
        public int Order { get; set; }

        public string QueueName
        {
            
            get
            {
                return string.Format(SyncDirection.Equals(Synchronization.SyncDirection.Export) 
                    ? "AftToSalsaQueue_{0}" 
                    : "SalsaToAftQueue_{0}", ObjectType);
            }
        }

        public string QueueHistoryName
        {
            get
            {
                return string.Format("{0}_History", QueueName);
            }
        }
        
    }
}
