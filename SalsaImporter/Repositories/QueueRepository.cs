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
        public const string QueueStatusExported = "Exported";
        public const string QueueStatusError = "Error";
        public const string QueueStatusSkipped = "SkippedIdenticalObject";
        

        private readonly IMapperFactory _mapperFactory;

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
        }


        public List<SyncObject> GetBatchOfObjects(string objectType, string tableName, int batchSize, int startKey)
        {

            var mapper = _mapperFactory.GetMapper(objectType);
            var sql = String.Format("SELECT top {0} * FROM {1} WHERE (Status != '{2}' OR Status IS NULL) AND Id > {3} ORDER BY Id", batchSize, tableName, QueueStatusError, startKey);
            var returnValue = new List<SyncObject>();
            using (var dataAdaptor = new SqlDataAdapter(sql, Config.DbConnectionString))
            {
                var dataSet = new DataSet();
                dataAdaptor.Fill(dataSet);

                var table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    var syncObject = new SyncObject(objectType);
                    foreach (DataColumn column in table.Columns)
                    {
                        SetValueToSyncObject(row, column, syncObject, mapper);
                    }
                    returnValue.Add(syncObject);
                }
            }

            return returnValue;
        }

        private  void SetValueToSyncObject(DataRow row, DataColumn column, SyncObject syncObject, IMapper mapper)
        {
            
            if (column.ColumnName == "Id")
            {
                syncObject.QueueId = Int32.Parse(row[column].ToString());
                return;
            }

            if (mapper.Mappings.All(m => !m.AftField.Equals(column.ColumnName))) return;
            var mapping = mapper.Mappings.First(m => m.AftField.EqualsIgnoreCase(column.ColumnName));
            syncObject[column.ColumnName] = DataTypeConverter.GetConverter(mapping.DataType).ReadAftValue(row[column]);
        }

     

        public void Dequeue(string tableName, int id)
        {
            ExecuteSql(String.Format("INSERT INTO {0}_History select * FROM {0} WHERE Id={1}", tableName, id));
            ExecuteSql(String.Format("DELETE FROM {0} WHERE id={1}", tableName, id));
        }

        public void UpdateStatus(string tableName, int id, string status, DateTime? processedDate = null)
        {
            ExecuteSql(String.Format("UPDATE {0} SET status='{1}' WHERE Id={2}", tableName, status, id));
            if(processedDate != null)
                ExecuteSql(String.Format("UPDATE {0} SET ProcessedDate='{1}' WHERE Id={2}", tableName, processedDate, id));
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
