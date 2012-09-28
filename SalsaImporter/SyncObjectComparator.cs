using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Mappers;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter
{
    public class SyncObjectComparator : ISyncObjectComparator
    {
        private ISalsaClient _salsaClient;

        public SyncObjectComparator(ISalsaClient salsaClient)
        {
            _salsaClient = salsaClient;
        }

        public bool AreIdentical(SyncObject aftObject, SyncObject salsaObject, List<FieldMapping> mappings)
        {
            if (salsaObject == null) return false;
            foreach (var mapping in mappings)
            {
                if (mapping.MappingRule.EqualsIgnoreCase(MappingRules.aftWins) 
                    && IsObjectDifferent(aftObject, salsaObject, mapping)) return false;
                if (!mapping.MappingRule.EqualsIgnoreCase(MappingRules.onlyIfBlank) || !IsSalsaFieldBlank(salsaObject, mapping)) continue;
                if (IsObjectDifferent(aftObject, salsaObject, mapping)) return false;
            }

            var membershipMapping = mappings.FirstOrDefault(m => m.MappingRule.EqualsIgnoreCase(MappingRules.writeOnlyNewMembership));
            if (membershipMapping == null || aftObject[membershipMapping.AftField] == null
                || aftObject[membershipMapping.AftField].Equals(0))
                return true;

            var groupType = membershipMapping.SalsaField.Split('_').First(); //membership field is  like "chapter_key"

            var doesMembershipExist = _salsaClient.DoesMembershipExist(groupType, aftObject.ObjectType, aftObject[membershipMapping.AftField].ToString(), salsaObject.SalsaKey.ToString());
            return doesMembershipExist;
        }


        private static bool IsSalsaFieldBlank(SyncObject salsaObject, FieldMapping fieldMapping)
        {
            if (fieldMapping.DataType.EqualsIgnoreCase(DataType.Boolean))
                return false; //blank cannot be applied to bool in salsa;
            return salsaObject[fieldMapping.AftField] == null;
        }

        private static bool IsObjectDifferent(SyncObject aftObject, SyncObject salsaObject, FieldMapping mapping)
        {
            var aftValue = aftObject[mapping.AftField];
            var salsValue = salsaObject[mapping.AftField];
            if (aftValue == null && salsValue == null) return false;
            if (aftValue == null) return true;
            return !aftValue.Equals(salsValue);
        }
    }
}
