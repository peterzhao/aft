using System.Collections.Specialized;
using System.Xml.Linq;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Mappers
{
    public interface IMapper
    {
        NameValueCollection ToNameValues(ISyncObject obj);
        ISyncObject ToObject(XElement xElement);
        string SalsaType { get; }

        DynamicSyncObject ToDynamicObject(XElement element);
    }
}