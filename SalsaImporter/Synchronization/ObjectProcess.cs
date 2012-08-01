using System;
using System.Collections.Generic;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ObjectProcess : IObjectProcess
    {
        private ISyncObjectRepository _externalRepository;
        private ISyncObjectRepository _localRepository;
        private ISyncErrorHandler _errorHandler;

        public ObjectProcess(ISyncObjectRepository localRepository, ISyncObjectRepository externalRepository, ISyncErrorHandler errorHandler)
        {
            _localRepository = localRepository;
            _externalRepository = externalRepository;
            _errorHandler = errorHandler;
        }

        public void ProcessPulledObject<T>(T externalObj) where T:ISyncObject
        {
            try
            {
                var localObj = _localRepository.GetByExternalKey<T> (externalObj.ExternalKey.Value);
                if (localObj == null)
                    AddToLocal(externalObj);
                else if(!externalObj.Equals(localObj))
                {
                    if (localObj.localModifiedDate < externalObj.ExternalModifiedDate)
                        _localRepository.Update(externalObj, localObj);
                }
            }catch(Exception ex)
            {
                _errorHandler.HandlePullObjectFailure(externalObj, ex);
            }
        }

        private void AddToLocal(ISyncObject externalObj)
        {
            _localRepository.Add(externalObj);
        }
    }
}