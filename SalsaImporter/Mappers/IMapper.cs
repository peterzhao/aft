using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Mappers
{
    public interface IMapper
    {
        NameValueCollection ToNameValues(SyncObject obj);
        SyncObject ToObject(XElement xElement);
     
    }
}