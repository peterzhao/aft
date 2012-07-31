using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface IObjectProcess
    {
         void ProcessPulledObjects(IEnumerable<ISyncObject> syncObjects);
    }
}