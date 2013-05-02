using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Mappers;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class SalsaRepository : ISalsaRepository
    {
        private readonly ISalsaClient _salsa;
        private readonly IMapperFactory _mapperFactory;
        private readonly ISyncErrorHandler _syncErrorHandler;
        private ISyncObjectComparator _objectComparator;

        public SalsaRepository(ISalsaClient salsa, IMapperFactory mapperFactory, ISyncErrorHandler syncErrorHandler, ISyncObjectComparator objectComparator)
        {
            _salsa = salsa;
            _mapperFactory = mapperFactory;
            _syncErrorHandler = syncErrorHandler;
            _objectComparator = objectComparator;
        }

        public IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey, DateTime minimumModifiedDate)
        {
           
            var mapper = _mapperFactory.GetMapper(objectType);
            List<string> salsaFields = mapper.Mappings.Select(mapping => mapping.SalsaField).ToList();

            try
            {
                var xElements = _salsa.GetObjects(objectType, batchSize, startKey, minimumModifiedDate, salsaFields);
                return MapToSyncObjects(objectType, xElements);
            }
            catch (SalsaClientException exception)
            {
                if (batchSize == 1)
                    return FindNextValidObject(objectType, startKey, minimumModifiedDate, exception, salsaFields);
 
                Logger.Debug(string.Format("Reducing batch size, getting again with batch size: {0} starting at {1}", batchSize / 2, startKey));
                return GetBatchOfObjects(objectType, batchSize / 2, startKey, minimumModifiedDate);
            }
        }

        private IEnumerable<SyncObject> FindNextValidObject(string objectType, int firstStartKey, DateTime minimumModifiedDate,
                                                SalsaClientException firstException, List<string> salsaFields)
        {
            SalsaClientException exception = firstException;
            int startKey = firstStartKey;
            
            for (;;)
            {
                startKey = _salsa.GetNextKey(objectType, startKey, minimumModifiedDate);

                _syncErrorHandler.HandleSalsaClientException(objectType, startKey, this, exception);
                Logger.Debug(string.Format("Skipped record {0}", startKey));

                try
                {
                    var xElements = _salsa.GetObjects(objectType, 1, startKey, minimumModifiedDate, salsaFields);
                    return MapToSyncObjects(objectType, xElements);
                } 
                catch (SalsaClientException newException)
                {
                    exception = newException;
                }
            }
        }

        private List<SyncObject> MapToSyncObjects(string objectType, List<XElement> xElements)
        {
            var mapper = _mapperFactory.GetMapper(objectType);

            var batchOfObjects = new List<SyncObject>();
            foreach (var element in xElements)
            {
                try
                {
                    var syncObject = mapper.ToAft(element);
                    batchOfObjects.Add(syncObject);
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Could not map {0}", element), ex);
                    _syncErrorHandler.HandleMappingFailure(objectType, element, this, ex);
                }
            }
            return batchOfObjects;
        }

        public bool Save(SyncObject aftObject)
        {
            try
            {
                var mapper = _mapperFactory.GetMapper(aftObject.ObjectType);
                var salsaFields = mapper.Mappings.Select(mapping => mapping.SalsaField).ToList();
                var primaryKeyMapping = mapper.PrimaryKeyMapping;
                Logger.Trace("got primaryKeyMapping" + primaryKeyMapping.AftField + ":" + primaryKeyMapping.SalsaField);
                var salsaXml = _salsa.GetObjectBy(aftObject.ObjectType, primaryKeyMapping.SalsaField,
                                                  aftObject[primaryKeyMapping.AftField].ToString(), salsaFields);
                Logger.Trace("got salsaXml");
                 var salsaObject = mapper.ToAft(salsaXml);
                if (_objectComparator.AreIdentical(aftObject, salsaObject, mapper.Mappings)) return false;
                Logger.Trace("Saving to salsa");
                aftObject.SalsaKey = int.Parse(_salsa.Save(aftObject.ObjectType, mapper.ToSalsa(aftObject, salsaObject)));
                return true;
            }
            catch(Exception ex)
            {
                Logger.Error("Got an error when saving object to salsa", ex);
                throw new SaveToSalsaException("Got error when saving object to Salsa.", ex);
            }
        }

        public SyncObject Get(string objectType, int key)
        {
            var xElement = _salsa.GetObject(objectType, key.ToString());
            var mapper = _mapperFactory.GetMapper(objectType);
            return mapper.ToAft(xElement);
        }

        public DateTime CurrentTime
        {
            get { return _salsa.CurrentTime; }
        }

    }
}
