using Microsoft.AspNetCore.Identity;
using Prezentex.Domain.Entities;
using Prezentex.Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories
{
    public class Seed
    {
        public static async Task SeedData(EntitiesDbContext context, UserManager<User> userManager)
        {
            if (context.Gifts.Any()) return;

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "Jefferson",
                    Email = "jefferson@gmail.com",
                    DisplayName = "Jeff"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "Stevie",
                    Email = "stevie@gmail.com",
                    DisplayName = "Steff"
                }
            };



            var recipients = new List<Recipient>
            {
                new Recipient
                {
                    Id= Guid.NewGuid(),
                    BirthDay = DateTime.UtcNow,
                    NameDay = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    Name = "Mark",
                    User = users.FirstOrDefault(),
                    Note = Guid.NewGuid().ToString()
                },
                new Recipient
                {
                    Id= Guid.NewGuid(),
                    BirthDay = DateTime.UtcNow.AddDays(13),
                    NameDay = DateTime.UtcNow.AddMonths(3),
                    CreatedDate = DateTime.UtcNow,
                    Name = "George",
                    User = users.FirstOrDefault(),
                    Note = Guid.NewGuid().ToString()
                }
            };

            var gifts = new List<Gift>
            {
                new Gift
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Description = "road bike",
                    Name = "Colnago C60",
                    Price = 35661.25M,
                    ProductUrl = "allegro.pl",
                    Recipients = new List<Recipient> { recipients.First() },
                    User = users.First()
                },
                new Gift
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Description = "speakers",
                    Name = "Sony MX4",
                    Price = 1456.29M,
                    ProductUrl = "amazon.pl",
                    Recipients = new List<Recipient> { recipients.First() },
                    User = users.First()
                }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
            await context.Recipients.AddRangeAsync(recipients);
            await context.Gifts.AddRangeAsync(gifts);
            await context.SaveChangesAsync();
        }
    }
}
