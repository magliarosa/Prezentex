using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prezentex.Domain.Entities;

namespace Prezentex.Infrastructure.Persistence.Repositories
{
    public class EntitiesDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<Guid>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
