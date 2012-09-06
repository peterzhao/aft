using System;
using System.Linq;
using System.Collections.Generic;
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
                   
                    var syncObject = mapper.ToObject(element);
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
                var salsaXml = _salsa.GetObject(syncObject.ObjectType, syncObject.SalsaKey.ToString());
                var salsaObject = mapper.ToObject(salsaXml);
                syncObject.SalsaKey = int.Parse(_salsa.Save(syncObject.ObjectType, mapper.ToNameValues(syncObject, salsaObject)));
                NotifySyncEvent(this, new SyncEventArgs {EventType = SyncEventType.Export, Destination = this, SyncObject = syncObject});
            }
            catch(Exception ex)
            {
                _syncErrorHandler.HandleSyncObjectFailure(syncObject, this, ex);
            }
        }

        public SyncObject Get(string objectType, int key)
        {
            var xElement = _salsa.GetObject(objectType, key.ToString());
            var mapper = _mapperFactory.GetMapper(objectType);
            return mapper.ToObject(xElement);
        }

        public DateTime CurrentTime
        {
            get { return _salsa.CurrentTime; }
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };
    }
}
