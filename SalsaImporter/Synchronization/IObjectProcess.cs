using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface IObjectProcess
    {
         void ProcessPulledObject(ISyncObject externalObj);
    }
}