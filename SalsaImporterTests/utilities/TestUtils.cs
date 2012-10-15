using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using SalsaImporter;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporterTests.Utilities
{
    internal class TestUtils
    {
        private static SalsaClient SalsaClient
        {
            get { return new SalsaClient(); }
        }

        public static SalsaRepository SalsaRepository
        {
            get { return new SalsaRepository(SalsaClient, new MapperFactory(), new SyncErrorHandler(10),
                new SyncObjectComparator(SalsaClient)); }
        }

        public static void RemoveAllSalsa(string objectType, bool fetchOnlyKeys = true)
        {
            SalsaClient.DeleteAllObjects(objectType, 100, fetchOnlyKeys);
        }

        public static void InsertToSalsa(params SyncObject[] objects)
        {
            objects.ToList().ForEach(o => SalsaRepository.Save(o));
        }

        public static void UpdateSalsa(params SyncObject[] objects)
        {
            objects.ToList().ForEach(o => SalsaRepository.Save(o));
        }

        public static void ClearAllSessions()
        {
            using (var db = new AftDbContext())
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
                db.Database.ExecuteSqlCommand("delete from SalsaToAftQueue_Supporter_History");
                db.Database.ExecuteSqlCommand("delete from AftToSalsaQueue_Supporter");
                db.Database.ExecuteSqlCommand("delete from AftToSalsaQueue_Supporter_History");
            }
        }

        public static void ExecuteSql(string sql)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public static List<Dictionary<string, object>> ReadAllFromTable(string tableName)
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

        public static void InsertSupporterToExportQueue(string email, string firstName, string lastName, DateTime? aft_Match_DateTime, string title, int salsaKey, int chapterKey = 0)
        {
            using (var db = new AftDbContext())
            {
                string sql = null;
                if(aft_Match_DateTime.HasValue)
                 sql = String.Format("INSERT INTO AftToSalsaQueue_Supporter ( Email, First_Name, Last_Name, AFT_Match_DateTime, Title, SalsaKey, Chapter_KEY) VALUES ('{0}', '{1}', '{2}', '{3}','{4}', {5}, {6})", email, firstName, lastName, aft_Match_DateTime, title, salsaKey, chapterKey);
                else
                    sql = String.Format("INSERT INTO AftToSalsaQueue_Supporter ( Email, First_Name, Last_Name, Title, SalsaKey, Chapter_KEY) VALUES ('{0}', '{1}', '{2}', '{3}','{4}', {5})", email, firstName, lastName, title, salsaKey, chapterKey);
                db.Database.ExecuteSqlCommand(sql);
            }
        }


        public static List<XElement> GetAllFromSalsa(string objectType)
        {
            var salsa = new SalsaClient();
            return salsa.GetObjects(objectType, 100, 0, new DateTime(1991,1,1));

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
            XElement xElement = salsaClient.GetObjectBy("custom_column", "name", name);
            if (type == xElement.StringValueOrNull("type"))
                return;

            salsaClient.CreateSupporterCustomColumn(customColumn);
        }

        public static void RemoveAllSyncConfigs()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete from syncEvents");
                db.Database.ExecuteSqlCommand("delete from fieldMappings");
                db.Database.ExecuteSqlCommand("delete from SyncConfigs");
            }
        }

        public static void CreateDefaultSyncConfigs()
        {
            RemoveAllSyncConfigs();
            using (var db = new AftDbContext())
            {
                db.SyncConfigs.Add(new SyncConfig {ObjectType = "supporter", SyncDirection = "export", Order = 1});
                db.SyncConfigs.Add(new SyncConfig {ObjectType = "supporter", SyncDirection = "import", Order = 2});
                db.SaveChanges();
            }
        }

        public static void CreateSyncConfig(string objectType, string syncDirection, int order)
        {
            using (var db = new AftDbContext())
            {
                db.SyncConfigs.Add(new SyncConfig { ObjectType = objectType, SyncDirection = syncDirection, Order = order });
                db.SaveChanges();
            }
        }

        public static void RemoveFieldMappings()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete from FieldMappings");
                db.SaveChanges();
            }
        }

        public static void CreateFieldMappings(FieldMapping fieldMapping)
        {
            using (var db = new AftDbContext())
            {
                db.FieldMappings.Add(fieldMapping);
                db.SaveChanges();
            }
        }

        public static void RecreateFieldMappingForTest()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete from FieldMappings");
                db.SaveChanges();

                new List<FieldMapping>
                    {
                        new FieldMapping{ObjectType = "supporter",AftField = "Title",SalsaField = "Title",
                                         DataType = "string",MappingRule = MappingRules.salsaWins},
                         new FieldMapping{ObjectType = "supporter",AftField = "Phone",SalsaField = "Phone",
                                         DataType = "string",MappingRule = MappingRules.aftWins},
                        new FieldMapping{ObjectType = "supporter",AftField = "First_Name",SalsaField = "First_Name",
                                         DataType = "string",MappingRule = MappingRules.onlyIfBlank},
                        new FieldMapping{ObjectType = "supporter",AftField = "Last_Name",SalsaField = "Last_Name",
                                         DataType = "string",MappingRule = MappingRules.onlyIfBlank},
                        new FieldMapping{ObjectType = "supporter",AftField = "Email",SalsaField = "Email",
                                         DataType = "string",MappingRule = MappingRules.primaryKey},
                        new FieldMapping{ObjectType = "supporter",AftField = "AFT_Match_DateTime",SalsaField = "cdb_match_date",
                                         DataType = "datetime",MappingRule = MappingRules.aftWins},
                        new FieldMapping{ObjectType = "supporter",AftField = "Chapter_KEY",SalsaField = "chapter_KEY",
                                         DataType = "int",MappingRule = MappingRules.writeOnlyNewMembership},
                        new FieldMapping{ObjectType = "chapter",AftField = "Name",SalsaField = "Name",
                                         DataType = "string",MappingRule = MappingRules.aftWins},
                        new FieldMapping{ObjectType = "supporter",AftField = "Last_Modified",SalsaField = "Last_Modified",
                                         DataType = "datetime",MappingRule = MappingRules.readOnly},
                    }.ForEach(f => db.FieldMappings.Add(f));

                db.SaveChanges();
            }
        }

        public static void RemoveSyncConfigForObjectType(string objectType)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(string.Format("delete from SyncConfigs where ObjectType='{0}'", objectType));
            }
        }

        public static void RemoveFieldMappingsForObjectType(string objectType)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(string.Format("delete from FieldMappings where ObjectType='{0}'", objectType));
            }
        }

        public static void CreateEmptyTable(string tableName)
        {
            DropTable(tableName);
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(string.Format("CREATE TABLE {0}([f1] [int])", tableName));
            }
        }

        public static void DropTable(string tableName)
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand(string.Format("IF OBJECT_ID('dbo.{0}', 'U') IS NOT NULL DROP TABLE dbo.{0}", tableName));
            }
        }
    }
}
