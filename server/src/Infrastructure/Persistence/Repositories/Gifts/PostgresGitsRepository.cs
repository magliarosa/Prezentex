using Prezentex.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Infrastructure.Persistence.Repositories.Gifts
{
    public class PostgresGitsRepository : IGiftsRepository
    {
        private readonly EntitiesDbContext context;
        public PostgresGitsRepository(EntitiesDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task CreateGiftAsync(Gift gift)
        {
            await context.AddAsync(gift);
            await context.SaveChangesAsync();
        }

        public async Task DeleteGiftAsync(Guid id)
        {
            var gift = await GetGiftAsync(id);
            context.Remove(gift);
            await context.SaveChangesAsync();
        }

        public async Task<Gift> GetGiftAsync(Guid id)
        {
            var gift = await context.Gifts.Where(gift => gift.Id == id).Include(gift => gift.Recipients).SingleOrDefaultAsync();
            return await Task.FromResult(gift);
        }

        public async Task<IEnumerable<Gift>> GetGiftsAsync()
        {
            var gifts = await context.Gifts.ToListAsync();
            return await Task.FromResult(gifts);
        }


        public async Task UpdateGiftAsync(Gift gift)
        {
            var existingGift = await context.Gifts.FindAsync(gift.Id);
            context.Entry(existingGift).CurrentValues.SetValues(gift);
            await context.SaveChangesAsync();
        }

        public async Task AddRecipientToGiftAsync(Guid giftId, Guid recipientId)
        {
            var gift = await context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var recipient = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            gift.Recipients.Add(recipient);
            await context.SaveChangesAsync();
        }

        public async Task RemoveRecipientFromGiftAsync(Guid giftId, Guid recipientId)
        {
            var gift = await context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var recipient = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            gift.Recipients.Remove(recipient);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UserOwnsGiftAsync(Guid giftId, Guid userId)
        {
            var gift = await context.Gifts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == giftId);

            if (gift == null)
                return false;

            return gift.UserId == userId;
        }
    }
}
