﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private const string SalsaKeyColumnName = "SalsaKey";

        public void Push(SyncObject syncObject, string tableName)
        {
            InsertToQueue(syncObject, tableName, syncObject.FieldNames);
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Add, Destination = this, SyncObject = syncObject });
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };

        private void InsertToQueue(SyncObject syncObject, string tableName, List<string> fields)
        {

            using (var connection = new SqlConnection(Config.DbConnectionString))
            {
                connection.Open();

                fields.Add(SalsaKeyColumnName);
                
                var columnNames = String.Join(",", fields);
                var parameterPlaceholders = String.Join(",", fields.Select(f => String.Format("@{0}", f)));
                var insertStatement = String.Format("INSERT {0} ( {1}) VALUES ({2});", tableName, columnNames,
                                                    parameterPlaceholders);

                var command = new SqlCommand(insertStatement, connection);

                var parameters = fields
                    .Where(f => f!=SalsaKeyColumnName)
                    .Select(f => new SqlParameter(f, syncObject[f])).ToList();
              
                parameters.Add(new SqlParameter(SalsaKeyColumnName, syncObject.Id));

                command.Parameters.AddRange(parameters.ToArray());

                command.ExecuteNonQuery();
            }
        }





//        public SyncObject NextFromQueue(string tableName, List<string> fields)
//        {
//            var fixedFields = new List<string> { IdColumnName };
//
//            var connString = Database.Connection.ConnectionString;
//
//            var columnNames = String.Join(",", fields.Union(fixedFields));
//
//            var sqlConnection = new SqlConnection(connString);
//            var sc = new SqlCommand(string.Format("SELECT TOP 1 {0} FROM {1}", columnNames, tableName), sqlConnection);
//
//            if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();
//
//            var item = new SyncObject("supporter");
//
//            var reader = sc.ExecuteReader();
//            if (!reader.Read())
//                return null;
//
//            item.Id = (int)reader[IdColumnName];
//
//            fields.ForEach(field => item[field] = (string)reader[field]);
//
//            if (sqlConnection.State != ConnectionState.Closed) sqlConnection.Close();
//
//            return item;
//        }
//
//        public void RemoveFromQueue(SyncObject item, string tableName)
//        {
//            var deleteStatement = String.Format("DELETE FROM {0} WHERE {1} = @{1};", tableName, IdColumnName);
//            var parameters = new SqlParameter[] { new SqlParameter(IdColumnName, item.Id) };
//            Database.ExecuteSqlCommand(deleteStatement, parameters);
//        }
    }
}
