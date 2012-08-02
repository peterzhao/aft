using System;
using System.Collections.Generic;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ObjectProcess : IObjectProcess
    {
        private readonly ISyncObjectRepository _externalRepository;
        private readonly ISyncObjectRepository _localRepository;
        private readonly ISyncErrorHandler _errorHandler;

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
                    if (localObj.LocalModifiedDate < externalObj.ExternalModifiedDate)
                        _localRepository.Update(externalObj, localObj);
                }
            }catch(Exception ex)
            {
                _errorHandler.HandlePullObjectFailure(externalObj, ex);
            }
        }

      

        public void ProcessPushingObject<T>(T localObj) where T:ISyncObject
        {
            try
            {
                T externalObj = default(T);
                if(localObj.ExternalKey.HasValue) externalObj  = _externalRepository.Get<T>(localObj.ExternalKey.Value);
                if (externalObj == null) 
                    AddToExternal(localObj);
                else if (!externalObj.Equals(localObj))
                {
                    if (localObj.LocalModifiedDate >= externalObj.ExternalModifiedDate)
                    {
                        _externalRepository.Update<T>(localObj, externalObj);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandlePushObjectFailure(localObj, ex);
            }

        }

        private void AddToExternal(ISyncObject localObj)
        {
            var newObject = localObj.Clone();
            newObject.ExternalKey = _externalRepository.Add(localObj);
            _localRepository.Update(newObject, localObj);

        }

        private void AddToLocal(ISyncObject externalObj)
        {
            _localRepository.Add(externalObj);
        }
    }
}