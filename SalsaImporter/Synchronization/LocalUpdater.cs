using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class LocalUpdater : IObjectUpdater
    {
        protected readonly ISyncObjectRepository _destination;
        private readonly ISyncErrorHandler _errorHandler;

        public LocalUpdater(ISyncObjectRepository destination, ISyncErrorHandler errorHandler)
        {
            _destination = destination;
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
                    var destinationKey = _destination.Add(destinationObject);
                    AfterCreateNew(destinationKey, sourceObject);
                }
                else if (!destinationObject.Equals(existingDestinationObject))
                {
                    destinationObject.Id = existingDestinationObject.Id;
                    _destination.Update(destinationObject);
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
            var existingDestinationObject = _destination.GetByExternalKey<T>(externalKey);
            return existingDestinationObject;
        }

        protected virtual void AfterCreateNew<T>(int destinationKey, T sourceObject) where T : class, ISyncObject
        {
        }

        private T GenerateDestinationObject<T>(T soureObject) where T : class, ISyncObject
        {
            var sourceObject = soureObject.Clone();
            sourceObject.Id = 0;
            sourceObject.ExternalId = soureObject.Id;
            return (T)sourceObject;
        }

    }
}