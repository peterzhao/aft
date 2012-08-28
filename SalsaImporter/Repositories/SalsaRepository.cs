using System;
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
            var xElements = _salsa.GetObjects(objectType, batchSize, startKey.ToString(), minimumModifiedDate);
            var batchOfObjects = new List<SyncObject>();
            foreach (var element in xElements)
            {
                try
                {
                    var mapper = _mapperFactory.GetMapper(objectType);
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

        public int Add(SyncObject syncObject)
        {
            IMapper mapper = _mapperFactory.GetMapper(syncObject.ObjectType);
            var id = int.Parse(_salsa.Create(syncObject.ObjectType, mapper.ToNameValues(syncObject)));
            syncObject.Id = id;
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Add, Destination = this, SyncObject = syncObject });
            return id;
        }

        public void Update(SyncObject newData)
        {
            var mapper = _mapperFactory.GetMapper(newData.ObjectType);
            _salsa.Update(newData.ObjectType, mapper.ToNameValues(newData));
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Update, Destination = this, SyncObject = newData });
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
