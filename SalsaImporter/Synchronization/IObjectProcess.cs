using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface IObjectProcess
    {
        void ProcessPulledObject<T>(T externalObj) where T : ISyncObject;
    }
}