using Microsoft.EntityFrameworkCore;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories.Users
{
    public class PostgresUsersRepository : IUsersRepository
    {
        private readonly EntitiesDbContext context;
        public PostgresUsersRepository(EntitiesDbContext context)
        {
            this.context = context;
        }

        public async Task AddGiftToUserAsync(Guid userId, Guid giftId)
        {
            var gift = await context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var user = await context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Gifts.Add(gift);
            await context.SaveChangesAsync();
        }

        public async Task AddRecipientToUserAsync(Guid userId, Guid recipientId)
        {
            var recipient = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            var user = await context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Recipients.Add(recipient);
            await context.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await context.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == id);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            var user = await context.Users.Include(user => user.Gifts).Include(user => user.Recipients).SingleOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await context.Users.Include(user => user.Gifts).Include(user => user.Recipients).SingleOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await context.Users.ToListAsync();
            return users;
        }

        public async Task RemoveGiftFromUserAsync(Guid userId, Guid giftId)
        {
            var gift = await context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var user = await context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Gifts.Remove(gift);
            await context.SaveChangesAsync();
        }

        public async Task RemoveRecipientFromUserAsync(Guid userId, Guid recipientId)
        {
            var recipient = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            var user = await context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Recipients.Remove(recipient);
            await context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await context.Users.FindAsync(user.Id);
            context.Entry(existingUser).CurrentValues.SetValues(user);
            await context.SaveChangesAsync();
        }
    }
}
