using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncEventArgs: EventArgs
    {
        public ISyncObject SyncObject { get; set; }
        public string EventType { get; set; }
        public ISyncObjectRepository Destination { get; set; }
        public Exception Error { get; set; }
    }
}