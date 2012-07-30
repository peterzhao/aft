using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SalsaImporter.Aft
{
    public class SupporterMapper
    {
        readonly string[] ignoredFields = new[] { "Last_Modified", "Date_Created", "Last_Bounce", "Id", "uid" };
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
            result.Add("uid", supporter.Id.ToString());
            return result;
        }

        public Supporter ToSupporter(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
