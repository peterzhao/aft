using System.Data.Entity;

namespace SalsaImporter.Aft
{
    public class AftDbContext : DbContext
    {
        public DbSet<Supporter> Supporters { get; set; }
    }
}