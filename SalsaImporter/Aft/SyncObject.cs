using System;
using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class SyncObject : ISyncObject 
    {
        public int Id { get; set; }
        public DateTime? ModifiedDate { get; private set; }
        public int? ExternalId { get; set; }
        private IDictionary<string, string> _dictionary;

        public SyncObject()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public void Add(String fieldName, String value)
        {
            _dictionary.Add(fieldName, value);
        }

        public string Get(String fieldName)
        {
            return _dictionary[fieldName];
        }

        public bool EqualValues(object other)
        {
            throw new NotImplementedException();
        }

        public ISyncObject Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string dictionaryFields = string.Join(", ", _dictionary.Select(pair => string.Format("{0}: {1}", pair.Key, pair.Value)));
            
            return string.Format("Id: {0}, ExternalId: {1}, ModifiedDate: {2}, {3}", 
                Id, 
                ExternalId, 
                ModifiedDate, 
                dictionaryFields);
        }
    }
}