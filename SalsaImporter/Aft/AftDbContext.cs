using System;
using System.Data.Entity;
using SalsaImporter.Synchronization;

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
    }
}