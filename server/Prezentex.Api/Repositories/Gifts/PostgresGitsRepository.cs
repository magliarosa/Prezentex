using Microsoft.EntityFrameworkCore;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories.Gifts
{
    public class PostgresGitsRepository : IUsersRepository
    {
        private readonly EntitiesDbContext dbContext;
        public PostgresGitsRepository(EntitiesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task CreateGiftAsync(Gift gift)
        {
            await dbContext.AddAsync(gift);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteGiftAsync(Guid id)
        {
            var gift = await GetGiftAsync(id);
            dbContext.Remove(gift);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Gift> GetGiftAsync(Guid id)
        {
            var gift = await dbContext.Gifts.Where(gift => gift.Id == id).SingleOrDefaultAsync();
            return await Task.FromResult(gift);
        }

        public async Task<IEnumerable<Gift>> GetGiftsAsync()
        {
            var gifts = await dbContext.Gifts.ToListAsync();
            return await Task.FromResult(gifts);
        }

        public async Task UpdateGiftAsync(Gift gift)
        {
            var existingGift = await dbContext.Gifts.FindAsync(gift.Id);
            dbContext.Entry(existingGift).CurrentValues.SetValues(gift);
            await dbContext.SaveChangesAsync();
        }
    }
}
