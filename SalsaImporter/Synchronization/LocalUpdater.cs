using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class LocalUpdater : IObjectUpdater
    {
        protected readonly ISyncObjectRepository _destinationRepository;
        private readonly ISyncErrorHandler _errorHandler;

        public LocalUpdater(ISyncObjectRepository destinationRepository, ISyncErrorHandler errorHandler)
        {
            _destinationRepository = destinationRepository;
            _errorHandler = errorHandler;
        }

        public void Update<T>(T sourceObject) where T:class, ISyncObject
        {
            var destinationObject = SwapKeys(sourceObject);
            try
            {
                var existingDestinationObject = FindExistingDestinationObject(sourceObject);
                if (existingDestinationObject == null)
                {
                    Logger.Trace(String.Format("New in  {0}: {1}", _destinationRepository, destinationObject));
                    var destinationKey = _destinationRepository.Add(destinationObject);
                    AfterCreateNew(destinationKey, sourceObject);
                }
                else if (!destinationObject.Equals(existingDestinationObject))
                {
                    destinationObject.Id = existingDestinationObject.Id;
                    Logger.Trace(String.Format("Updating {0}: existing: {1}, \n new: {2}", _destinationRepository, existingDestinationObject, destinationObject) );
                    _destinationRepository.Update(destinationObject);
                }
            }
            catch(Exception ex)
            {       
               _errorHandler.HandleSyncObjectFailure(destinationObject, _destinationRepository, ex);
            }
        }

        protected virtual T FindExistingDestinationObject<T>(T sourceObject) where T : class, ISyncObject
        {
            var externalKey = sourceObject.Id;
            var existingDestinationObject = _destinationRepository.GetByExternalKey<T>(externalKey);
            return existingDestinationObject;
        }

        protected virtual void AfterCreateNew<T>(int destinationKey, T sourceObject) where T : class, ISyncObject
        {
        }

        private T SwapKeys<T>(T sourceObject) where T : class, ISyncObject
        {
            var destinationObject = sourceObject.Clone();
            destinationObject.Id = 0;
            destinationObject.ExternalId = sourceObject.Id;
            return (T)destinationObject;
        }

    }
}