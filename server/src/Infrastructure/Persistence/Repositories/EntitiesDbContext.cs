using Microsoft.EntityFrameworkCore;
using Prezentex.Domain.Entities;

namespace Prezentex.Infrastructure.Persistence.Repositories
{
    public class EntitiesDbContext : DbContext
    {
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
