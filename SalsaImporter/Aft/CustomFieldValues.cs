using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Utilities;

namespace SalsaImporter.Aft
{
    public class CustomFieldValues
    {
        private readonly List<SupporterCustomField> _fields;
        private readonly ICollection<SupporterCustomFieldValue> _values;

        public CustomFieldValues(List<SupporterCustomField> fields, ICollection<SupporterCustomFieldValue> values)
        {
            _fields = fields;
            _values = values;
        }

        public object this[string fieldName]
        {
            get
            {
                var field = GetField(fieldName);
                var fieldValue = FindFieldValue(field);
                if (fieldValue == null)
                    return null;
                return fieldValue.Value;
            }
            set
            {
                var field = GetField(fieldName);
                var fieldValue = FindFieldValue(field);
                if (fieldValue == null)
                {
                    fieldValue = new SupporterCustomFieldValue {SupporterCustomField_Id= field.Id, Value = value.ToString()};
                    _values.Add(fieldValue);
                }else
                {
                    fieldValue.Value = value == null? null: value.ToString();
                }
            }
        }

        public IEnumerable<String> Names
        {
            get { return _values.Select(v => _fields.First(f => f.Id == v.SupporterCustomField_Id).Name); }
        } 

        protected bool Equals(CustomFieldValues other)
        {
            if (this._values.Count != other._values.Count) return false;
            return _values.All(myValue => other._values.Any(
                other_value => other_value.SupporterCustomField_Id == myValue.SupporterCustomField_Id 
                && StringUtility.EqualsIncludingNullEmpty(other_value.Value, myValue.Value)));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CustomFieldValues) obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Join(" ", _values
                .Select(v => string.Format("{0}:{1}", _fields.First(f => f.Id == v.SupporterCustomField_Id).Name, v.Value)));
        }

        private SupporterCustomFieldValue FindFieldValue(SupporterCustomField field)
        {
            var fieldValue = _values.FirstOrDefault(v => v.SupporterCustomField_Id == field.Id);
            return fieldValue;
        }

        private SupporterCustomField GetField(string fieldName)
        {
            var field = _fields.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
                throw new ApplicationException(string.Format("{0} is not valid.", fieldName));
            return field;
        }
    }
}
