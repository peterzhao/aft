using System.Data.Entity;
using SalsaImporter.Mappers;

namespace SalsaImporter.Synchronization
{
    public class AftDbContext : DbContext
    {
        public AftDbContext() : base(Config.DbConnectionString){}

        public DbSet<SessionContext> SessionContexts { get; set; }
        public DbSet<JobContext> JobContexts { get; set; }
        public DbSet<FieldMapping> FieldMappings { get; set; }
        public DbSet<SyncConfig> SyncConfigs { get; set; }
    }
}