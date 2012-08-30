using System;
using System.Xml.Linq;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncEventArgs: EventArgs
    {
        public object Destination { get; set; }

        public string EventType { get; set; }
        public Exception Error { get; set; }

     
        public string Data { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }

        public SyncObject SyncObject
        {
            set { 
                ObjectType = value.ObjectType;
                Data = value.ToString();
                ObjectId = value.Id;
            }
        }
    }
}