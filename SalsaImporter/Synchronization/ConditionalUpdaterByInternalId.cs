using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ConditionalUpdaterByInternalId : IConditonalUpdater
    {
        private readonly ISyncObjectRepository _destination;
        private readonly ISyncObjectRepository _source;
        private readonly ISyncErrorHandler _errorHandler;

        public ConditionalUpdaterByInternalId(ISyncObjectRepository destination,
            ISyncObjectRepository source,
            ISyncErrorHandler errorHandler)
        {
            _destination = destination;
            _source = source;
            _errorHandler = errorHandler;
        }

        public void MaybeUpdate<T>(T sourceObject) where T:class, ISyncObject
        {
            var destinationKey = sourceObject.ExternalId;
            var destinationObject = ForDestinationRepository(sourceObject);
            try
            {
                var existingDestinationObject = destinationKey == null ? null : _destination.Get<T>((int)destinationKey);
                if (existingDestinationObject == null)
                {
                    destinationKey = _destination.Add(destinationObject);
                    
                    // Remember the destination id in the source's external id.
                    sourceObject.ExternalId = destinationKey;
                    _source.Update(sourceObject);
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