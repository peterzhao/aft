using System;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public interface IQueueRepository
    {
        void Push(SyncObject syncObject, string tableName);
        event EventHandler<SyncEventArgs> NotifySyncEvent;
    }
}