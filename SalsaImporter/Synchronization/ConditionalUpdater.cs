using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ConditionalUpdater : IConditonalUpdater
    {
        protected readonly ISyncObjectRepository _destination;
        private readonly ISyncErrorHandler _errorHandler;

        public ConditionalUpdater(ISyncObjectRepository destination, ISyncErrorHandler errorHandler)
        {
            _destination = destination;
            _errorHandler = errorHandler;
        }

        public void MaybeUpdate<T>(T sourceObject) where T:class, ISyncObject
        {
            var destinationObject = ForDestinationRepository(sourceObject);
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

        private T ForDestinationRepository<T>(T incomingSourceObject) where T : class, ISyncObject
        {
            var sourceObject = incomingSourceObject.Clone();
            sourceObject.Id = 0;
            sourceObject.ExternalId = incomingSourceObject.Id;
            return (T)sourceObject;
        }

    }
}