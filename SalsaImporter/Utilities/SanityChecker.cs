﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Utilities
{
    public class SanityChecker
    {
        Dictionary<string, string> _typeMappings = new Dictionary<string, string>
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
        public List<string> Verify()
        {
            var result = new List<string>();
            using (var db = new AftDbContext())
            {
                List<SyncConfig> jobConfigs = db.SyncConfigs.ToList();
                jobConfigs.ForEach(job =>
                                       {
                                           if (DbUtility.IsTableExist(job.QueueName))
                                               VerifyFields(db, job, result);
                                           else   
                                               result.Add(string.Format("Could not find the table for the queue {0}", job.QueueName));
                                           
                                           if (!DbUtility.IsTableExist(job.QueueHistoryName))
                                               result.Add(string.Format("Could not find the history table {0}", job.QueueHistoryName));
                                       });
            }
            return result;
        }

        private void VerifyFields(AftDbContext db, SyncConfig job, List<string> result)
        {
            var mappings = db.FieldMappings.Where(m => m.ObjectType == job.ObjectType).ToList();
            var columns = DbUtility.GetColumnsInfo(job.QueueName);

            mappings.ForEach(mapping =>
                                 {
                                     if (columns.Any(column => CanMatch(column, mapping, job)))
                                         return;
                                     result.Add(String.Format("Could not find column {0} as {1} in queue {2}", mapping.AftField, mapping.DataType, job.QueueName));
                                 });
            

        }

        private bool CanMatch(DataColumn column, FieldMapping mapping, SyncConfig job)
        {
            if (job.SyncDirection.EqualsIgnoreCase(SyncDirection.Export) 
                && mapping.MappingRule.EqualsIgnoreCase(MappingRules.readOnly)) 
                return true;

            if (job.SyncDirection.EqualsIgnoreCase(SyncDirection.Import)
               && mapping.MappingRule.EqualsIgnoreCase(MappingRules.writeOnly))
                return true;

            if (!_typeMappings.Keys.Contains(column.DataType.Name))
            {
                Logger.Debug("cannot find type:" + column.DataType.Name);
                return false;
            }

            var match = column.ColumnName.EqualsIgnoreCase(mapping.AftField) 
                && _typeMappings[column.DataType.Name].EqualsIgnoreCase(mapping.DataType);
            
            return match;
        }


    }
}