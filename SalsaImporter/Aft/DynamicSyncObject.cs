using System;
using System.Dynamic;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class DynamicSyncObject : DynamicObject, ISyncObject 
    {
        public int Id { get; set; }
        public DateTime? ModifiedDate { get; private set; }
        public int? ExternalId { get; set; }

      
        public bool EqualValues(object other)
        {
            throw new NotImplementedException();
        }


        public ISyncObject Clone()
        {
            throw new NotImplementedException();
        }


    }
}