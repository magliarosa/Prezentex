using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Infrastructure.Persistence.Repositories.Users
{
    public class PostgresUsersRepository : IUsersRepository
    {
        private readonly EntitiesDbContext _context;
        private readonly UserManager<User> _userManager;

        public PostgresUsersRepository(EntitiesDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddGiftToUserAsync(Guid userId, Guid giftId)
        {
            var gift = await _context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Gifts.Add(gift);
            await _context.SaveChangesAsync();
        }

        public async Task AddRecipientToUserAsync(Guid userId, Guid recipientId)
        {
            var recipient = await _context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Recipients.Add(recipient);
            await _context.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _userManager.CreateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            var user = await _context.Users.Include(user => user.Gifts).Include(user => user.Recipients).SingleOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.Include(user => user.Gifts).Include(user => user.Recipients).SingleOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task RemoveGiftFromUserAsync(Guid userId, Guid giftId)
        {
            var gift = await _context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRecipientFromUserAsync(Guid userId, Guid recipientId)
        {
            var recipient = await _context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            user.Recipients.Remove(recipient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
        }
    }
}
