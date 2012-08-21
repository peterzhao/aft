using System;
using System.ComponentModel.DataAnnotations;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class SupporterGroup : ISyncObject
    {
        public int Id { get; set; }
        public int? ExternalId { get; set; }
       

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDate { get; set; }

        public int SupporterId { get; set; }
        public int GroupId { get; set; }

        protected bool Equals(SupporterGroup other)
        {
            return GroupId == other.GroupId && SupporterId == other.SupporterId;
        }

        public override bool Equals(object obj)
        {
            return EqualValues(obj);
        }

       

        public bool EqualValues(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SupporterGroup)obj);
        }

        public ISyncObject Clone()
        {
            return new SyncObjectCloner().Clone(this);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (GroupId*397) ^ SupporterId;
            }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, ExternalId: {1}, ModifiedDate: {2}, SupporterId: {3}, GroupId: {4}", Id, ExternalId, ModifiedDate, SupporterId, GroupId);
        }
    }
}