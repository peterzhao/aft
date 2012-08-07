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
            var destinationObject = GenerateDestinationObject(sourceObject);
            try
            {
                var existingDestinationObject = FindExistingDestinationObject(sourceObject);
                if (existingDestinationObject == null)
                {
                    var destinationKey = _destinationRepository.Add(destinationObject);
                    AfterCreateNew(destinationKey, sourceObject);
                }
                else if (!destinationObject.Equals(existingDestinationObject))
                {
                    destinationObject.Id = existingDestinationObject.Id;
                    _destinationRepository.Update(destinationObject);
                }
            }
            catch(Exception ex)
            {       
               _errorHandler.HandleAddObjectFailure(destinationObject, ex);
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

        private T GenerateDestinationObject<T>(T sourceObject) where T : class, ISyncObject
        {
            var destinationObject = sourceObject.Clone();
            destinationObject.Id = 0;
            destinationObject.ExternalId = sourceObject.Id;
            return (T)destinationObject;
        }

    }
}