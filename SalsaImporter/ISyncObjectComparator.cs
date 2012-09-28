using System.Collections.Generic;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public interface ISyncObjectComparator
    {
        bool AreIdentical(SyncObject aftObjct, SyncObject salsaObject, List<FieldMapping> mappings);
    }
}