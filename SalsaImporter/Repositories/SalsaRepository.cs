﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SalsaImporter.Mappers;
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

        public IEnumerable<T> GetBatchOfObjects<T>(int batchSize, int startKey, DateTime lastProcessedDateTime) where T : ISyncObject
        {
            var xElements = _salsa.GetObjects(GetObjectType<T>(), batchSize, startKey.ToString(), lastProcessedDateTime, null);
            var mapper = GetMapper<T>();
            return xElements.Select(element => (T) mapper.ToObject(element));
        }

        public int Add<T>(T syncObject) where T : ISyncObject
        {
            return int.Parse(_salsa.Create(GetObjectType<T>(), GetMapper<T>().ToNameValues(syncObject)));
        }

        public void Update<T>(T newData, T oldData) where T : ISyncObject
        {
            _salsa.Update(GetObjectType<T>(), GetMapper<T>().ToNameValues(newData));
        }

        public T GetByExternalKey<T>(int key) where T:ISyncObject
        {
            var objectType = GetObjectType<T>();
            var xElement = _salsa.GetObject(key.ToString(), objectType);
            var mapper = _mapperFactory.GetMapper<T>();
            return (T)mapper.ToObject(xElement);
        }

       

        public T GetByLocallKey<T>(int key) where T:ISyncObject
        {
            throw new NotImplementedException();
        }

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
