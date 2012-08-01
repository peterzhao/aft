using System;
using System.Collections.Generic;

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

        public void ProcessPulledObject(ISyncObject externalObj)
        {
            try
            {
                var localObj = _localRepository.GetByExternalKey(externalObj.ExternalKey.Value);
                if (localObj == null)
                    AddToLocal(externalObj);
                else
                {
                    if (localObj.localModifiedDate > externalObj.ExternalModifiedDate)
                        _externalRepository.Update(localObj, externalObj);
                    else
                        _localRepository.Update(externalObj, localObj);
                }
            }catch(Exception ex)
            {
                _errorHandler.HandlePullObjectFailure(externalObj, ex);
            }
        }

        private void AddToLocal(ISyncObject externalObj)
        {
            ISyncObject newObj = externalObj.Clone();
            newObj.LocalKey = _localRepository.Add(externalObj);
            _externalRepository.Update(newObj, externalObj);
        }
    }
}