using System.Data.Entity;

namespace SalsaImporter.Aft
{
    public class AftDbContext : DbContext
    {
        public DbSet<Supporter> Supporters { get; set; }
        public DbSet<SyncRun> SyncRuns { get; set; }
    }
}