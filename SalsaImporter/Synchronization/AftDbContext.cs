using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using SalsaImporter.Mappers;

namespace SalsaImporter.Synchronization
{
    public class AftDbContext : DbContext
    {
        public AftDbContext() : base(Config.DbConnectionString){}

        public DbSet<SessionContext> SessionContexts { get; set; }
        public DbSet<JobContext> JobContexts { get; set; }
        public DbSet<SyncEvent> SyncEvents { get; set; }
        public DbSet<FieldMapping> FieldMappings { get; set; }
    }
}