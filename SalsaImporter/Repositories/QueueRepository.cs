using System;
using System.Collections.Generic;
using System.Data;
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
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Import, Destination = this, SyncObject = syncObject });
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };

        public List<SyncObject> DequequBatchOfObjects(string objectType, string tableName, int batchSize, int startKey)
        {
            var sql = string.Format("SELECT top {0} * FROM {1} where Id > {2} order by Id", batchSize, tableName, startKey);
            Logger.Debug(sql);
            using (var dataAdaptor = new SqlDataAdapter(sql,Config.DbConnectionString))
            {
                var dataSet = new DataSet();
                dataAdaptor.Fill(dataSet);
                var returnValue = new List<SyncObject>();

                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    var data = new SyncObject(objectType);
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.ColumnName == "Id")
                            data.Id = int.Parse(row[column].ToString());
                        else
                            data[column.ColumnName] = row[column].ToString();
                    }
                    returnValue.Add(data);
                }
                return returnValue;
            }
        }

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
