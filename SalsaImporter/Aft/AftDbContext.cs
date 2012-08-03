using System;
using System.Data.Entity;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class AftDbContext : DbContext
    {
        public DbSet<Supporter> Supporters { get; set; }
        public DbSet<SessionContext> SessionContexts { get; set; }
        public DbSet<JobContext> JobContexts { get; set; }
        public DbSet<T> Records<T>() where T : class
        {
            return Set<T>();
        }
    }
}