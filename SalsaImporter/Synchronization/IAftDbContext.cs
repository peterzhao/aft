using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface IAftDbContext
    {
        void InsertToQueue(SyncObject syncObject, string tableName, List<string> fields);
    }
}