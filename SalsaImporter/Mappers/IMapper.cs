using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Mappers
{
    public interface IMapper
    {
        NameValueCollection ToSalsa(SyncObject aftObject, SyncObject salsaObject);
        bool IsIdentical(SyncObject aftObject, SyncObject salsaObject);
        SyncObject ToAft(XElement xElement);
        FieldMapping PrimaryKeyMapping { get; }
        List<FieldMapping> Mappings { get; }
    }
}