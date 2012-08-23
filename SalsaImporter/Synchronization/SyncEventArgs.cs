using System;
using System.Xml.Linq;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncEventArgs: EventArgs
    {
        private string _data;
        private int _objectId;
        private int _externalId;
        private string _objectType;
        public string EventType { get; set; }
        public ISyncObjectRepository Destination { get; set; }
        public Exception Error { get; set; }

        public ISyncObject SyncObject { 
            set {
                _data = value.ToString();
                _objectId = value.Id;
                _externalId = value.ExternalId.HasValue ? value.ExternalId.Value : 0;
                _objectType = value.GetType().Name;
            } 
        }

        public void InitializeWithUnmappedObject(String objectType, XElement unmappedObject)

        {
            _objectType = objectType;
            _data = unmappedObject.ToString();    
        }

        public string Data
        {
            get { return _data; }
        }

        public int ObjectId
        {
            get { return _objectId; }
        }

        public int ExternalId

        {
            get { return _externalId; }
        }

        public string ObjectType
        {
            get { return _objectType; }
        }
    }
}