using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Mappers
{
    public class SupporterMapper : IMapper
    {
        Dictionary<string, string> map = new Dictionary<string, string>
                                             {
                                                {"supporter_KEY","supporter_KEY"},
                                                //{"Last_Modified","Last_Modified"},
                                                //{"Date_Created","Date_Created"},
                                                {"Title","Title"},
                                                {"First_Name","First_Name"},
                                                {"MI","MI"},
                                                {"Last_Name","Last_Name"},
                                                {"Suffix","Suffix"},
                                                {"Email","Email"},
                                                {"Email_Preference","Email_Preference"},
                                                //{"Last_Bounce","Last_Bounce"},
                                                {"Phone","Phone"},
                                                {"Cell_Phone","Cell_Phone"},
                                                {"Phone_Provider","Phone_Provider"},
                                                {"Work_Phone","Work_Phone"},
                                                {"Pager","Pager"},
                                                {"Home_Fax","Home_Fax"},
                                                {"Work_Fax","Work_Fax"},
                                                {"Street","Street"},
                                                {"Street_2","Street_2"},
                                                {"Street_3","Street_3"},
                                                {"City","City"},
                                                {"State","State"},
                                                {"Zip","Zip"},
                                                {"PRIVATE_Zip_Plus_4","PRIVATE_Zip_Plus_4"},
                                                {"County","County"},
                                                {"District","District"},
                                                {"Country","Country"},
                                                {"Latitude","Latitude"},
                                                {"Longitude","Longitude"},
                                                {"Organization","Organization"},
                                                {"Department","Department"},
                                                {"Occupation","Occupation"},
                                                {"Instant_Messenger_Service","Instant_Messenger_Service"},
                                                {"Instant_Messenger_Name","Instant_Messenger_Name"},
                                                {"Web_Page","Web_Page"},
                                                {"Alternative_Email","Alternative_Email"},
                                                {"Other_Data_1","Other_Data_1"},
                                                {"Other_Data_2","Other_Data_2"},
                                                {"Other_Data_3","Other_Data_3"},
                                                {"Notes","Notes"},
                                                {"Source","Source"},
                                                {"Source_Details","Source_Details"},
                                                {"Source_Tracking_Code","Source_Tracking_Code"},
                                                {"Tracking_Code","Tracking_Code"},
                                                {"Status","Status"},
                                                {"uid","uid"},
                                                {"Timezone","Timezone"},
                                                {"Language_Code","Language_Code"},
                                                {"CustomString0","CustomString0"},
                                                {"CustomString1","CustomString1"},
                                                {"CustomString2","CustomString2"},
                                                {"CustomString3","CustomString3"},
                                                {"CustomString4","CustomString4"},
                                                {"CustomString5","CustomString5"},
                                                {"CustomString6","CustomString6"},
                                                {"CustomString7","CustomString7"},
                                                {"CustomString8","CustomString8"},
                                                {"CustomString9","CustomString9"},
                                                {"CustomInteger0","CustomInteger0"},
                                                {"CustomInteger1","CustomInteger1"},
                                                {"CustomInteger2","CustomInteger2"},
                                                {"CustomInteger3","CustomInteger3"},
                                                {"CustomInteger4","CustomInteger4"},
                                                {"CustomBoolean0","CustomBoolean0"},
                                                {"CustomBoolean1","CustomBoolean1"},
                                                {"CustomBoolean2","CustomBoolean2"},
                                                {"CustomBoolean3","CustomBoolean3"},
                                                {"CustomBoolean4","CustomBoolean4"},
                                                {"CustomBoolean5","CustomBoolean5"},
                                                {"CustomBoolean6","CustomBoolean6"},
                                                {"CustomBoolean7","CustomBoolean7"},
                                                {"CustomBoolean8","CustomBoolean8"},
                                                {"CustomBoolean9","CustomBoolean9"},
                                                {"CustomDateTime0","CustomDateTime0"},
                                             }; 
        public NameValueCollection ToNameValues(ISyncObject supporter)
        {

            var result = new NameValueCollection();
            foreach (var property in supporter.GetType().GetProperties())
            {
                var propertyName = property.Name;
                if(map.Keys.Contains(propertyName))
                {
                    object value = property.GetValue(supporter, null);
                    if(value != null)
                        result.Add(map[propertyName], value.ToString());
                }
            }
            return result;
        }

        public ISyncObject ToObject(XElement element) 
        {
            var supporter = new Supporter();
            foreach (var property in supporter.GetType().GetProperties())
            {
                var propertyName = property.Name;
                var propertyType = property.PropertyType;
                object propertyValue = null;
                if (!map.Keys.Contains(propertyName)) continue;
                if (propertyType == typeof(String))
                    propertyValue = element.StringValueOrNull(map[propertyName]);
                else if (propertyType == typeof(int?))
                    propertyValue = element.IntValueOrNull(map[propertyName]);
                else if (propertyType == typeof(float?))
                    propertyValue = element.FloatValueOrNull(map[propertyName]);
                else if (propertyType == typeof(DateTime?))
                    propertyValue = element.DateTimeValueOrNull(map[propertyName]);
                if (propertyValue != null)
                    property.SetValue(supporter, propertyValue, null);
            }
            return supporter;
        }
    }
}
