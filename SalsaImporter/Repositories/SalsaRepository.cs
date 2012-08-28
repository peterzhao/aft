using System;
using System.Collections.Generic;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class SalsaRepository : ISyncObjectRepository
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

        public IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime minimumModifiedDate) where T : class, ISyncObject
        {
            var mapper = _mapperFactory.GetMapper(typeof (T).Name);
            var xElements = _salsa.GetObjects(mapper.SalsaType, batchSize, startKey.ToString(), minimumModifiedDate);
            var batchOfObjects = new List<T>();
            foreach (var element in xElements)
            {
                try
                {
                    var syncObject = (T) mapper.ToObject(element);
                    batchOfObjects.Add(syncObject);
                } 
                catch (Exception ex )
                {
                    Logger.Error(string.Format("Could not map {0}", element), ex); 
                    _syncErrorHandler.HandleMappingFailure(typeof(T).Name, element, this, ex);
                }
            }
            return batchOfObjects;
        }

        public IEnumerable<SyncObject> GetBatchOfObjects(string objectType, int batchSize, int startKey, DateTime minimumModifiedDate)
        {
            var mapper = _mapperFactory.GetMapper(objectType);
            var xElements = _salsa.GetObjects(mapper.SalsaType, batchSize, startKey.ToString(), minimumModifiedDate);
            var batchOfObjects = new List<SyncObject>();
            foreach (var element in xElements)
            {
                try
                {
                    var syncObject = mapper.ToSyncObject(element);
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

        public int Add<T>(T syncObject) where T : class, ISyncObject
        {
            IMapper mapper = _mapperFactory.GetMapper(typeof (T).Name);
            var id = int.Parse(_salsa.Create(mapper.SalsaType, mapper.ToNameValues(syncObject)));
            syncObject.Id = id;
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Add, Destination = this, SyncObject = syncObject });
            return id;
        }

        public void Update<T>(T newData) where T : class, ISyncObject
        {
            var mapper = _mapperFactory.GetMapper(typeof (T).Name);
            _salsa.Update(mapper.SalsaType, _mapperFactory.GetMapper(typeof (T).Name).ToNameValues(newData));
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Update, Destination = this, SyncObject = newData });
        }

        public T GetByExternalKey<T>(int key) where T : class, ISyncObject
        {
            throw new NotImplementedException();
        }

        public T Get<T>(int key) where T : class, ISyncObject
        {
            var mapper = _mapperFactory.GetMapper(typeof (T).Name);
            var objectType = mapper.SalsaType;
            var xElement = _salsa.GetObject(objectType, key.ToString());
            return (T)mapper.ToObject(xElement);
        }

        public DateTime CurrentTime
        {
            get { return _salsa.CurrentTime; }
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };
    }
}
