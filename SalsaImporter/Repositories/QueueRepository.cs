using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private IMapperFactory _mapperFactory;

        public QueueRepository(IMapperFactory mapperFactory)
        {
            _mapperFactory = mapperFactory;
        }

        public void Push(SyncObject syncObject, string tableName)
        {
            InsertToQueue(syncObject, tableName, syncObject.FieldNames);
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Import, Destination = this, SyncObject = syncObject });
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };

        public List<SyncObject> DequequBatchOfObjects(string objectType, string tableName, int batchSize, int startKey)
        {
            var mapper = _mapperFactory.GetMapper(objectType);
            var aftFields = mapper.Mappings.Select(m => m.AftField).ToList();

            var sql = string.Format("SELECT top {0} * FROM {1} where Id > {2} order by Id", batchSize, tableName, startKey);
            var returnValue = new List<SyncObject>();
            using (var dataAdaptor = new SqlDataAdapter(sql, Config.DbConnectionString))
            {
                var dataSet = new DataSet();
                dataAdaptor.Fill(dataSet);

                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    var syncObject = new SyncObject(objectType);
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.ColumnName == "Id")
                            syncObject.Id = int.Parse(row[column].ToString());
                        else if (aftFields.Contains( column.ColumnName))
                            syncObject[column.ColumnName] = row[column];
                    }
                    returnValue.Add(syncObject);
                }
            }
            if ( returnValue.Count > 0 ) 
                ExecuteSql(string.Format("delete from {0} where Id <= {1}", tableName, returnValue.Last().Id));

            return returnValue;

        }

        private void ExecuteSql(string sql)
        {
            using(var connection = new SqlConnection(Config.DbConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }

        private void InsertToQueue(SyncObject syncObject, string tableName, List<string> fields)
        {

            using (var connection = new SqlConnection(Config.DbConnectionString))
            {
                connection.Open();

                
                var columnNames = String.Join(",", fields);
                var parameterPlaceholders = String.Join(",", fields.Select(f => String.Format("@{0}", f)));
                var insertStatement = String.Format("INSERT {0} ( {1}) VALUES ({2});", tableName, columnNames,
                                                    parameterPlaceholders);

                var command = new SqlCommand(insertStatement, connection);

                var parameters = fields
                    .Select(f => new SqlParameter(f, syncObject[f])).ToList();
              

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
