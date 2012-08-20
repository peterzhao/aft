using System;

namespace SalsaImporter.Synchronization
{
    public interface ISyncObject 
    {
         int Id { get; set; }
         DateTime? ModifiedDate { get; }
         int? ExternalId { get; set; }
    }
}