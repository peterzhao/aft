using System;
using System.ComponentModel.DataAnnotations;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class Group : ISyncObject
    {
        public int Id { get; set; }
        public int? ExternalId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDate { get; set; }

        public string Name { get; set; }

        public string ReferenceName { get; set; }

        //public string ParentId { get; set; }

        public string Description { get; set; }

        public string Notes { get; set; }

        protected bool Equals(Group other)
        {
            return string.Equals(Name, other.Name) && string.Equals(ReferenceName, other.ReferenceName) &&
                   string.Equals(Description, other.Description) && string.Equals(Notes, other.Notes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Group) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ReferenceName != null ? ReferenceName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Notes != null ? Notes.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Id: {0}, ExternalId: {1}, ModifiedDate: {2}, GroupName: {3}, ReferenceName: {4}, Description: {5}, Notes: {6}",
                    Id, ExternalId, ModifiedDate, Name, ReferenceName, Description, Notes);
        }
    }
}