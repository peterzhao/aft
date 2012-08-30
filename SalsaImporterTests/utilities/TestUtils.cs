using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Utilities
{
    class TestUtils
    {

        private static SalsaClient SalsaClient
        {
            get { return new SalsaClient(); }
        }

        public static SalsaRepository SalsaRepository
        {
            get { return new SalsaRepository(SalsaClient, new MapperFactory(), new SyncErrorHandler(10)); }
        }

        public static void RemoveAllSalsa(string objectType)
        {
            SalsaClient.DeleteAllObjects(objectType, 100, true);
        }
 
        public static void InsertToSalsa(params SyncObject[] objects)
        {
            objects.ToList().ForEach(syncObject => SalsaRepository.Save(syncObject));
        }

        public static void UpdateSalsa(params SyncObject[] objects) 
        {
            objects.ToList().ForEach(SalsaRepository.Save);
        }

        public static void ClearAllSessions()
        {
            using(var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete  from SyncEvents");
                db.Database.ExecuteSqlCommand("delete  from JobContexts");
                db.Database.ExecuteSqlCommand("delete  from SessionContexts");
            }
        }

        public static void ClearAllQueues()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete from SalsaToAftQueue_Supporter");
                db.Database.ExecuteSqlCommand("delete from AftToSalsaQueue_Supporter");
            }
        }

        public static void ExecuteSql(string sql)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public static List<Dictionary<string, object>> ReadAllFromQueue(string tableName)
        {

            using (var dataAdaptor = new SqlDataAdapter(string.Format("SELECT * FROM {0}", tableName),
                                                 Config.DbConnectionString))
            {
                var dataSet = new DataSet();
                dataAdaptor.Fill(dataSet);
                var returnValue = new List<Dictionary<string, object>>();

                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    var data = new Dictionary<string, object>();
                    foreach (DataColumn column in table.Columns)
                    {
                        data[column.ColumnName] = row[column];
                    }
                    returnValue.Add(data);
                }
                return returnValue;
            }
        }

        public static void InsertSupporterToExportQueue(string email, string firstName, string lastName)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(string.Format("INSERT INTO AftToSalsaQueue_Supporter ( Email, First_Name, Last_Name, SalsaKey) VALUES ('{0}', '{1}', '{2}', 0)", email, firstName, lastName));
            }
        }

        public static List<XElement> GetAllFromSalsa(string objectType)
        {

            var salsa = new SalsaClient();
            return salsa.GetObjects(objectType, 100, "0", new DateTime(1991,1,1));
        }
    }
}
