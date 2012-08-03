using System;
using System.Collections.Generic;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ConditionalUpdater : IConditonalUpdater
    {
        private readonly ISyncObjectRepository _destination;
        private readonly ISyncErrorHandler _errorHandler;

        public ConditionalUpdater(ISyncObjectRepository destination, ISyncErrorHandler errorHandler)
        {
            _destination = destination;
            _errorHandler = errorHandler;
        }

        public void MaybeUpdate<T>(T sourceObject) where T:class, ISyncObject
        {
            try
            {
                var externalKey = sourceObject.ExternalKey;
                var localObj = _destination.GetByExternalKey<T> (externalKey.Value);
                if (localObj == null)
                {
                    _destination.Add<T>(sourceObject);
                }
                else if (!sourceObject.Equals(localObj))
                {
                    if (localObj.LocalModifiedDate < sourceObject.ExternalModifiedDate)
                        _destination.Update(sourceObject, localObj);
                }
            }
            catch(Exception ex)
            {       
               _errorHandler.HandlePullObjectFailure(sourceObject, ex);
            }
        }
    }
}