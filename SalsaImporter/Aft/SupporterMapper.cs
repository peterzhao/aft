using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SalsaImporter.Aft
{
    public class SupporterMapper
    {
        readonly string[] ignoredFields = new []{"Last_Modified", "Date_Created", "Last_Bounce", "Id"};
        public NameValueCollection ToNameValues(Supporter supporter)
        {
            var result = new NameValueCollection();
            foreach (var property in supporter.GetType().GetProperties())
            {
                if(!ignoredFields.Contains(property.Name))
                {
                    object value = property.GetValue(supporter, null);
                    if(value != null)
                        result.Add(property.Name, value.ToString());
                }
            }
            result.Add("aft_id", supporter.Id.ToString());
            return result;
        }
    }
}
