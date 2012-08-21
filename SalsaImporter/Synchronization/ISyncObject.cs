using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncObject 
    {
         int Id { get; set; }
         DateTime? ModifiedDate { get; }
         int? ExternalId { get; set; }

         ISyncObject Clone();
         bool EqualValues(object other);
         //void CopyFrom(ISyncJob other);
    }
}