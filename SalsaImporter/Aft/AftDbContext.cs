using System;
using System.Collections.Generic;
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
        public AftDbContext():base(Config.DbConnectionString){}

        public DbSet<Supporter> Supporters { get; set; }
        public DbSet<SessionContext> SessionContexts { get; set; }
        public DbSet<JobContext> JobContexts { get; set; }
        public DbSet<SyncEvent> SyncEvents { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<SupporterCustomField> SupporterCustomFields { get; set; }

        public void InsertToQueue(XElement syncObject, string tableName, List<string> fields)
        {
            var columnNames = String.Join(",", fields);
            var parameterPlaceholders = String.Join(",", fields.Select(f => String.Format("@{0}", f)));
            var insertStatement = String.Format("INSERT {0} ({1}) VALUES ({2});", tableName, columnNames, parameterPlaceholders);

            var parameters = fields.Select(f => new SqlParameter(f, syncObject.StringValueOrNull(f))).ToArray();

            Console.WriteLine(insertStatement);

            Database.ExecuteSqlCommand(insertStatement, parameters);
        }
    }
}