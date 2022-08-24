using Microsoft.EntityFrameworkCore;
using Prezentex.Entities;

namespace Prezentex.Repositories
{
    public class EntitiesDbContext : DbContext
    {
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options):
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
    }
}
