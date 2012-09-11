using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private IMapperFactory _mapperFactory;

        public QueueRepository(IMapperFactory mapperFactory)
        {
            _mapperFactory = mapperFactory;
        }

        public void Enqueue(string tableName, SyncObject syncObject)
        {
            List<string> fields = syncObject.FieldNames;
            using (var connection = new SqlConnection(Config.DbConnectionString))
            {
                connection.Open();
                var columnNames = String.Join(",", fields);
                var parameterPlaceholders = String.Join(",", fields.Select(f => String.Format("@{0}", f)));
                var insertStatement = String.Format("INSERT {0} ({1}) VALUES ({2});", tableName, columnNames,
                                                    parameterPlaceholders);

                var command = new SqlCommand(insertStatement, connection);
                var parameters = fields
                    .Select(f => new SqlParameter(f, syncObject[f])).ToList();
                command.Parameters.AddRange(parameters.ToArray());
                command.ExecuteNonQuery();
            }
            NotifySyncEvent(this, new SyncEventArgs { EventType = SyncEventType.Import, Destination = this, SyncObject = syncObject });
        }

        public event EventHandler<SyncEventArgs> NotifySyncEvent = delegate { };

        public List<SyncObject> GetBatchOfObjects(string objectType, string tableName, int batchSize, int startKey)
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
                        {
                            syncObject.QueueId = int.Parse(row[column].ToString());
                        }
                        else if (aftFields.Contains(column.ColumnName) && row[column] != DBNull.Value)
                        {
                            syncObject[column.ColumnName] = row[column];
                        }
                    }
                    returnValue.Add(syncObject);
                }
            }

            return returnValue;

        }

        public void Dequeue(string tableName, int id)
        {
            ExecuteSql(string.Format("insert into {0}_History select * from {0} where Id={1}", tableName, id));
            ExecuteSql(string.Format("delete from {0} where id={1}", tableName, id));
        }

        public void UpdateStatus(string tableName, int id, string status, DateTime? processedDate = null)
        {
            ExecuteSql(string.Format("update  {0} set status='{1}' where Id={2}", tableName, status, id));
            if(processedDate != null)
                ExecuteSql(string.Format("update  {0} set ProcessedDate='{1}' where Id={2}", tableName, processedDate, id));
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
    }
}
