using System;
using System.Xml.Linq;
using SalsaImporter.Aft;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class SyncEventArgs: EventArgs
    {
        private string _data;
        private int _objectId;
        private string _objectType;
        public string EventType { get; set; }
        public object Destination { get; set; }
        public Exception Error { get; set; }

        public SyncObject SyncObject { 
            set {
                _data = value.ToString();
                _objectId = value.Id;
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

        public string ObjectType
        {
            get { return _objectType; }
        }
    }
}