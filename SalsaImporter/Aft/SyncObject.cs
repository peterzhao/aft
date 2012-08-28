using System;
using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Aft
{
    public class SyncObject
    {
        private readonly string _objectType;
        public int Id { get; set; }
        public DateTime? ModifiedDate { get; private set; }

        public string ObjectType
        {
            get { return _objectType; }
        }

        private IDictionary<string, string> _dictionary;

        public SyncObject(string objectType)
        {
            _objectType = objectType;
            _dictionary = new Dictionary<string, string>();
        }

        public object this[string fieldName]
        {
            get { return null; }
            set
            {
                
            }
        }

        public void Set(String fieldName, String value)
        {
            _dictionary.Add(fieldName, value);
        }

        public string Get(String fieldName)
        {
            return _dictionary[fieldName];
        }

        public override string ToString()
        {
            string dictionaryFields = string.Join(", ", _dictionary.Select(pair => string.Format("{0}: {1}", pair.Key, pair.Value)));
            
            return string.Format("Id: {0},  ModifiedDate: {1}, {2}", 
                Id,  
                ModifiedDate, 
                dictionaryFields);
        }
    }
}