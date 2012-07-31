using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public class ObjectProcess : IObjectProcess
    {
        private ISyncObjectRepository _externalRepository;
        private ISyncObjectRepository _localRepository;

        public ObjectProcess(ISyncObjectRepository localRepository, ISyncObjectRepository externalRepository)
        {
            _localRepository = localRepository;
            _externalRepository = externalRepository;
        }

        public void ProcessPulledObjects(IEnumerable<ISyncObject> syncObjects)
        {
        }

      
    }
}