using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SalsaImporter.Aft
{
    public class SupporterCustomFieldValue
    {
        public int Id { get; set; }
        public virtual  Supporter Supporter { get; set; }

        [ForeignKey("SupporterCustomField_Id")]
        public virtual  SupporterCustomField SupporterCustomField { get; set; }
        public int SupporterCustomField_Id { get; set; }

        public string Value { get; set; }

        protected bool Equals(SupporterCustomFieldValue other)
        {
            return string.Equals(Value, other.Value) && SupporterCustomField_Id == other.SupporterCustomField_Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SupporterCustomFieldValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0)*397) ^ SupporterCustomField_Id;
            }
        }
    }
}
