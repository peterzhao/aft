using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ConditionalUpdaterByInternalId : ConditionalUpdater
    {
        private readonly ISyncObjectRepository _source;

        public ConditionalUpdaterByInternalId(ISyncObjectRepository destination,
            ISyncObjectRepository source,
            ISyncErrorHandler errorHandler) : base(destination, errorHandler)
        {
            _source = source;
        }

        protected override void AfterCreateNew<T>(int destinationKey, T sourceObject)
        {
            base.AfterCreateNew(destinationKey, sourceObject);
            
            sourceObject.ExternalId = destinationKey;
            _source.Update(sourceObject);
        }

        protected override T FindExistingDestinationObject<T>(T sourceObject)
        {
            var destinationKey = sourceObject.ExternalId;
            var existingDestinationObject = destinationKey == null ? null : _destination.Get<T>((int)destinationKey);
            return existingDestinationObject;
        }
    }
}