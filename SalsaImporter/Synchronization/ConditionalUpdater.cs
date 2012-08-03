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

        public void MaybeUpdate<T>(T incomingSourceObject) where T:class, ISyncObject
        {
            var externalKey = incomingSourceObject.Id;
            var sourceObject = ForDestinationRepository(incomingSourceObject);
            try
            {
                var existingDestinationObject = _destination.GetByExternalKey<T>(externalKey);
                if (existingDestinationObject == null)
                {
                    _destination.Add(sourceObject);
                }
                else if (!sourceObject.Equals(existingDestinationObject))
                {
                    sourceObject.Id = existingDestinationObject.Id;
                    _destination.Update(sourceObject);
                }
            }
            catch(Exception ex)
            {       
               _errorHandler.HandlePullObjectFailure(sourceObject, ex);
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