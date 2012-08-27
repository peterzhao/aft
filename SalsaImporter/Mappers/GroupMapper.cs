using System.Collections.Generic;
using SalsaImporter.Aft;

namespace SalsaImporter.Mappers
{
    public class GroupMapper : GenericMapper<Group>
    {
        public override string SalsaType
        {
            get
            {
                return "groups";
            }
        }
        protected override Dictionary<string, string> Map
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {"Id", "key"}, 
                               {"ExternalId", "external_ID"},
                               {"Name", "Group_Name"},
                               {"ReferenceName", "Reference_Name"},
                               {"Description", "Description"},
                               {"Notes", "Notes"}
                           };
            }
        }
    }
}