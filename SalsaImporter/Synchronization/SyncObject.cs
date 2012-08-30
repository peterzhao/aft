using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class SyncObject
    {
        private readonly IDictionary<string, object> _dictionary;
        private readonly string _objectType;

        public SyncObject(string objectType)
        {
            _objectType = objectType;
            _dictionary = new Dictionary<string, object>();
        }

        public int Id { get; set; }

        public string ObjectType
        {
            get { return _objectType; }
        }

        public List<string> FieldNames
        {
            get { return _dictionary.Keys.ToList(); }
        }

        public object this[string fieldName]
        {
            get { return _dictionary[fieldName]; }
            set { _dictionary[fieldName] = value; }
        }

        public override string ToString()
        {
            string dictionaryFields = string.Join(", ",
                                                  _dictionary.Select(
                                                      pair => string.Format("{0}: {1}", pair.Key, pair.Value.ToString())));

            return string.Format("Id: {0}, {1}",
                                 Id,
                                 dictionaryFields);
        }
    }
}