using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Utilities
{
    public class SanityChecker
    {
        private ISalsaClient _salsaClient;
        readonly Dictionary<string, string> _typeMappings = new Dictionary<string, string>
                                                       {
                                                           {"Int32", DataType.Int},
                                                           {"Int64", DataType.Int},
                                                           {"Int", DataType.Int},
                                                           {"String", DataType.String},
                                                           {"Double", DataType.Float},
                                                           {"Decimal", DataType.Float},
                                                           {"Boolean", DataType.Boolean},
                                                           {"DateTime", DataType.DateTime},
                                                       };

        readonly List<FieldMapping> _standardFieldMappings = new List<FieldMapping>
                                          {
                                              new FieldMapping{AftField = "Id", DataType = DataType.Int},
                                              new FieldMapping{AftField = "SalsaKey", DataType = DataType.Int},
                                              new FieldMapping{AftField = "CDate", DataType = DataType.DateTime},
                                              new FieldMapping{AftField = "ProcessedDate", DataType = DataType.DateTime},
                                              new FieldMapping{AftField = "Status", DataType = DataType.String},
                                          };

        public SanityChecker(ISalsaClient salsaClient)
        {
            _salsaClient = salsaClient;
        }

        public List<string> VerifyQueues()
        {
            var result = new List<string>();
            using (var db = new AftDbContext())
            {
                var jobConfigs = db.SyncConfigs.ToList();
                jobConfigs.ForEach(job =>
                                       {
                                           if (DbUtility.IsTableExist(job.QueueName))
                                               VerifyFields(db, job.ObjectType, job.QueueName, job.SyncDirection, result);
                                           else   
                                               result.Add(string.Format("Could not find the table for the queue {0}", job.QueueName));
                                           
                                           if (DbUtility.IsTableExist(job.QueueHistoryName))
                                               VerifyFields(db, job.ObjectType, job.QueueHistoryName, job.SyncDirection, result);
                                           else
                                               result.Add(string.Format("Could not find the history table {0}", job.QueueHistoryName));
                                       });

            }
            return result;
        }

        public List<string> VerifySalsaFields()
        {
            var result = new List<string>();
            using (var db = new AftDbContext())
            {
                db.SyncConfigs.GroupBy(job => job.ObjectType).Select(g => g.Key).ToList()
                    .ForEach(objectType => VerifySalsaFields(db, objectType, result));

            }
            return result;
        } 

        private void VerifySalsaFields(AftDbContext db, string objectType, List<string> result)
        {
            var mappings = db.FieldMappings.Where(m => m.ObjectType == objectType).ToList();
            if (_salsaClient.CountObjects(objectType) == 0) return; //do not verify if there is no record on salsa
            var fields = _salsaClient.GetFieldList(objectType);
            var missingFields = mappings.Where(m => !fields.Contains(m.SalsaField)).Select(m => m.SalsaField).ToList();
            if(missingFields.Count > 0)
              result.Add(string.Format("Could not find field(s) {0} of {1} from Salsa", string.Join(",", missingFields), objectType));
        }

        private void VerifyFields(AftDbContext db, string objectType, string queueName, string syncDirection, List<string> result)
        {
            var mappings = db.FieldMappings.Where(m => m.ObjectType == objectType).ToList();
            var columns = DbUtility.GetColumnsInfo(queueName);
            mappings.AddRange(_standardFieldMappings);
            mappings.ForEach(mapping =>
                                 {
                                     if (columns.Any(column => CanMatch(column, mapping, syncDirection)))
                                         return;
                                     result.Add(String.Format("Could not find column {0} as {1} in queue {2}", mapping.AftField, mapping.DataType, queueName));
                                 });
            

        }

        private bool CanMatch(DataColumn column, FieldMapping mapping, string syncDirection)
        {
            if (syncDirection.EqualsIgnoreCase(SyncDirection.Export) 
                && mapping.MappingRule.EqualsIgnoreCase(MappingRules.readOnly)) 
                return true;

            if (syncDirection.EqualsIgnoreCase(SyncDirection.Import)
               && mapping.MappingRule.EqualsIgnoreCase(MappingRules.writeOnly))
                return true;

            if (!_typeMappings.Keys.Contains(column.DataType.Name))
            {
                Logger.Debug("Cannot find type:" + column.DataType.Name);
                return false;
            }

            return column.ColumnName.EqualsIgnoreCase(mapping.AftField) 
                   && _typeMappings[column.DataType.Name].EqualsIgnoreCase(mapping.DataType);
        }
    }
}