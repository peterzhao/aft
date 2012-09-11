using System;
using System.Linq;
using System.Collections.Generic;
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

        public SalsaRepository(ISalsaClient salsa, IMapperFactory mapperFactory, ISyncErrorHandler syncErrorHandler)
        {
            _salsa = salsa;
            _mapperFactory = mapperFactory;
            _syncErrorHandler = syncErrorHandler;
        }

        public IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey, DateTime minimumModifiedDate)
        {
            var mapper = _mapperFactory.GetMapper(objectType);
            List<string> salsaFields = mapper.Mappings.Select(mapping => mapping.SalsaField).ToList();
            
            var xElements = _salsa.GetObjects(objectType, batchSize, startKey.ToString(), minimumModifiedDate, salsaFields);
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

        public void Save(SyncObject syncObject)
        {
            try
            {
                IMapper mapper = _mapperFactory.GetMapper(syncObject.ObjectType);
                var primaryKeyMapping = mapper.PrimaryKeyMapping;
                var salsaXml = _salsa.GetObjectBy(syncObject.ObjectType, primaryKeyMapping.SalsaField, syncObject[primaryKeyMapping.AftField].ToString());
                var salsaObject = mapper.ToAft(salsaXml);
                syncObject.SalsaKey = int.Parse(_salsa.Save(syncObject.ObjectType, mapper.ToSalsa(syncObject, salsaObject)));
                NotifySyncEvent(this, new SyncEventArgs {EventType = SyncEventType.Export, Destination = this, SyncObject = syncObject});
            }
            catch(Exception ex)
            {
                throw new SaveToSalsaException("Got error when save object into Salsa.", ex);
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

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };
    }
}
