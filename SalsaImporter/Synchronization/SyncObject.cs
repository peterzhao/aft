using System;
using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class SyncObject
    {
        public const string SalsaKeyColumnName = "SalsaKey";
        private readonly IDictionary<string, object> _dictionary;
        private readonly string _objectType;

        public SyncObject(string objectType)
        {
            _objectType = objectType;
            _dictionary = new Dictionary<string, object>();
            this[SalsaKeyColumnName] = 0;
        }

        public int QueueId { get; set; }

        public string ObjectType
        {
            get { return _objectType; }
        }

        public int SalsaKey
        {
            get
            {
                return (int)this[SalsaKeyColumnName];
            }
            set { this[SalsaKeyColumnName] = value; }
        }

        public List<string> FieldNames
        {
            get { return _dictionary.Keys.ToList(); }
        }

        public object this[string fieldName]
        {
            get
            {
                return !_dictionary.ContainsKey(fieldName) ? null : _dictionary[fieldName];
            }
            set { _dictionary[fieldName] = value; }
        }

        public override string ToString()
        {
            string dictionaryFields = String.Join(", ",
                                                  _dictionary.Select(
                                                      pair => String.Format("{0}: {1}", pair.Key, pair.Value.ToString())));

            return String.Format("Id: {0}, {1}",
                                 QueueId,
                                 dictionaryFields);
        }

    }
}