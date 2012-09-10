using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using SalsaImporter.Utilities;

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

        public static void RemoveAllSalsa(string objectType, bool fetchOnlyKeys = true)
        {
            SalsaClient.DeleteAllObjects(objectType, 100, fetchOnlyKeys);
        }

        public static void InsertToSalsa(params SyncObject[] objects)
        {
            objects.ToList().ForEach(SalsaRepository.Save);
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

            using (var dataAdaptor = new SqlDataAdapter(String.Format("SELECT * FROM {0}", tableName),
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

        public static void InsertSupporterToExportQueue(string email, string firstName, string lastName, DateTime aft_Match_DateTime, string title, int salsaKey, int chapterKey = 0)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(String.Format(
                    "INSERT INTO AftToSalsaQueue_Supporter ( Email, First_Name, Last_Name, AFT_Match_DateTime, Title, SalsaKey, Chapter_KEY) VALUES ('{0}', '{1}', '{2}', '{3}','{4}', {5}, {6})", email, firstName, lastName, aft_Match_DateTime, title, salsaKey, chapterKey));
            }
        }

     

        public static List<XElement> GetAllFromSalsa(string objectType)
        {
            var salsa = new SalsaClient();
            return salsa.GetObjects(objectType, 100, "0", new DateTime(1991,1,1));
        }

        public static void EnsureSupporterCustomColumn(string name, string type)
        {
            var salsaClient = new SalsaClient();
            var customColumn = new NameValueCollection
                                   {
                                       {"name", name},
                                       {"label", name},
                                       {"type", type}
                                   };
            var xElement = salsaClient.GetObjectBy("custom_column", "name", name);
            if (type == xElement.StringValueOrNull("type"))
                return;

            salsaClient.CreateSupporterCustomColumn(customColumn);
        }
    }
}
