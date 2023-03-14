using Microsoft.EntityFrameworkCore;
using Prezentex.Domain.Entities;

namespace Prezentex.Infrastructure.Persistence.Repositories
{
    public class CosmosDbContext : DbContext
    {
        public CosmosDbContext(DbContextOptions<CosmosDbContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
            modelBuilder.Entity<Notification>()
                .ToContainer("Notifications")
                .HasPartitionKey(x => x.Id);
        }

        public DbSet<Notification> Notifications { get; set; }


    }
}
