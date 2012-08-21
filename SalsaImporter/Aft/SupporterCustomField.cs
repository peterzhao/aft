using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

namespace SalsaImporter.Aft
{
    public class SupporterCustomField 
    {
        private static List<SupporterCustomField> _all;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<SupporterCustomFieldValue> SupporterCustomFieldValues { get; set; }

        public static List<SupporterCustomField> All
        {
            get
            {
                if(_all == null)
                {
                    using(var db = new AftDbContext())
                    {
                        _all = db.SupporterCustomFields.ToList();
                    }
                }
                
                return _all;
            }
          
        }
    }
}
