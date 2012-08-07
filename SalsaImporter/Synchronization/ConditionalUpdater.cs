using System;
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
            var externalKey = sourceObject.Id;
            var destinationObject = ForDestinationRepository(sourceObject);
            try
            {
                var existingDestinationObject = _destination.GetByExternalKey<T>(externalKey);
                if (existingDestinationObject == null)
                {
                    _destination.Add(destinationObject);
                }
                else if (!destinationObject.Equals(existingDestinationObject))
                {
                    destinationObject.Id = existingDestinationObject.Id;
                    _destination.Update(destinationObject);
                }
            }
            catch(Exception ex)
            {       
               _errorHandler.HandleObjectFailure(destinationObject, ex);
            }
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