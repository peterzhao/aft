using System;
using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class SalsaRepository : ISyncObjectRepository
    {
        private readonly ISalsaClient _salsa;
        private readonly IMapperFactory _mapperFactory;

        public SalsaRepository(ISalsaClient salsa, IMapperFactory mapperFactory)
        {
            _salsa = salsa;
            _mapperFactory = mapperFactory;
        }

        public IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime minimumModifiedDate) where T : class, ISyncObject
        {
            var xElements = _salsa.GetObjects(GetObjectType<T>(), batchSize, startKey.ToString(), minimumModifiedDate, null);
            var mapper = GetMapper<T>();
            return xElements.Select(element => (T) mapper.ToObject(element));
        }

        public int Add<T>(T syncObject) where T : class, ISyncObject
        {
            var id = int.Parse(_salsa.Create(GetObjectType<T>(), GetMapper<T>().ToNameValues(syncObject)));
            syncObject.Id = id;
            NotifySyncEvent(this, new SyncEventArgs{EventType = SyncEventType.Add, Destination = this, SyncObject = syncObject});
            return id;
        }

        public void Update<T>(T newData) where T : class, ISyncObject
        {
            _salsa.Update(GetObjectType<T>(), GetMapper<T>().ToNameValues(newData));
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Update, Destination = this, SyncObject = newData });
        }

        public T GetByExternalKey<T>(int key) where T: class, ISyncObject
        {
            throw new NotImplementedException();
        }

        public T Get<T>(int key) where T: class, ISyncObject
        {
            var objectType = GetObjectType<T>();
            var xElement = _salsa.GetObject(objectType, key.ToString());
            var mapper = _mapperFactory.GetMapper<T>();
            return (T)mapper.ToObject(xElement);
        }

        public DateTime CurrentTime
        {
            get { return _salsa.CurrentTime; }
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };

        private IMapper GetMapper<T>() where T : ISyncObject
        {
            return _mapperFactory.GetMapper<T>();
        }
        private string GetObjectType<T>()
        {
            return typeof(T).Name.ToLower();
        }
    }
}
