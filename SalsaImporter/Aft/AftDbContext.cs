﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Aft
{
    public class AftDbContext : DbContext
    {
        private const string IdColumnName = "ID";

        public AftDbContext():base(Config.DbConnectionString){}

        public DbSet<Supporter> Supporters { get; set; }
        public DbSet<SessionContext> SessionContexts { get; set; }
        public DbSet<JobContext> JobContexts { get; set; }
        public DbSet<SyncEvent> SyncEvents { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<SupporterCustomField> SupporterCustomFields { get; set; }

        public void InsertToQueue(DynamicSyncObject syncObject, string tableName, List<string> fields)
        {
            var columnNames = String.Join(",", fields);
            var parameterPlaceholders = String.Join(",", fields.Select(f => String.Format("@{0}", f)));
            var insertStatement = String.Format("INSERT {0} ({1}) VALUES ({2});", tableName, columnNames, parameterPlaceholders);

            var parameters = fields.Select(f => new SqlParameter(f, syncObject.Get(f))).ToArray();

            Database.ExecuteSqlCommand(insertStatement, parameters);
        }

        public DynamicSyncObject NextFromQueue(string tableName, List<string> fields)
        {
            var fixedFields = new List<string> { IdColumnName }; 
            
            var connString = Database.Connection.ConnectionString;

            var columnNames = String.Join(",", fields.Union(fixedFields));
            
            var sqlConnection = new SqlConnection(connString);
            var sc = new SqlCommand(string.Format("SELECT TOP 1 {0} FROM {1}", columnNames, tableName), sqlConnection);
            
            if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

            var item = new DynamicSyncObject();

            var reader = sc.ExecuteReader();
            if (! reader.Read() )
                return null;

            item.Id = (int)reader[IdColumnName];
            fields.ForEach(field => item.Add(field, (string)reader[field]));

            if (sqlConnection.State != ConnectionState.Closed) sqlConnection.Close();

            return item;
        }

        public void RemoveFromQueue(DynamicSyncObject item, string tableName)
        {
            var deleteStatement = String.Format("DELETE FROM {0} WHERE {1} = @{1};", tableName, IdColumnName);
            var parameters = new SqlParameter[] {new SqlParameter(IdColumnName, item.Id)};
            Database.ExecuteSqlCommand(deleteStatement, parameters);   
        }
    }
}