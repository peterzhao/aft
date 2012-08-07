using System;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class ExternalUpdater : LocalUpdater
    {
        private readonly ISyncObjectRepository _sourceRepository;

        public ExternalUpdater(ISyncObjectRepository destinationRepository,
            ISyncObjectRepository sourceRepository,
            ISyncErrorHandler errorHandler) : base(destinationRepository, errorHandler)
        {
            _sourceRepository = sourceRepository;
        }

        protected override void AfterCreateNew<T>(int destinationKey, T sourceObject)
        {
            base.AfterCreateNew(destinationKey, sourceObject);
            
            sourceObject.ExternalId = destinationKey;
            _sourceRepository.Update(sourceObject);
        }

        protected override T FindExistingDestinationObject<T>(T sourceObject)
        {
            var destinationKey = sourceObject.ExternalId;
            var existingDestinationObject = destinationKey == null ? null : _destinationRepository.Get<T>((int)destinationKey);
            return existingDestinationObject;
        }
    }
}