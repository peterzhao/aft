using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface IObjectProcess
    {
        void ProcessPulledObject<T>(T externalObj) where T : ISyncObject;
        void ProcessPushingObject<T>(T localObj) where T : ISyncObject;
    }
}