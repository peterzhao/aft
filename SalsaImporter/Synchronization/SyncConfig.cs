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
        
    }
}
