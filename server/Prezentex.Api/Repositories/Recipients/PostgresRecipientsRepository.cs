using Microsoft.EntityFrameworkCore;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories.Recipients
{
    public class PostgresRecipientsRepository : IRecipientsRepository
    {
        private readonly EntitiesDbContext context;
        public PostgresRecipientsRepository(EntitiesDbContext context)
        {
            this.context = context;
        }

        public async Task AddGiftToRecipientAsync(Guid recipientId, Guid giftId)
        {
            var gift = await context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var recipient = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            recipient.Gifts.Append(gift);
            await context.SaveChangesAsync();
        }

        public async Task CreateRecipientAsync(Recipient recipient)
        {
            await context.AddAsync(recipient);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRecipientAsync(Guid recipientId)
        {
            var recipientToDelete = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            context.Remove(recipientToDelete);
            await context.SaveChangesAsync();
        }

        public async Task<Recipient> GetRecipientAsync(Guid id)
        {
            var recipient = await context.Recipients.Where(x => x.Id == id).SingleOrDefaultAsync();
            return await Task.FromResult(recipient);
        }

        public async Task<IEnumerable<Recipient>> GetRecipientsAsync()
        {
            var recipients = await context.Recipients.ToArrayAsync();
            return await Task.FromResult(recipients);
        }

        public async Task RemoveGiftFromRecipientAsync(Guid recipientId, Guid giftId)
        {
            var gift = await context.Gifts.Where(x => x.Id == giftId).SingleOrDefaultAsync();
            var recipient = await context.Recipients.Where(x => x.Id == recipientId).SingleOrDefaultAsync();
            recipient.Gifts = recipient.Gifts.Where(x => x.Id != gift.Id);
            await context.SaveChangesAsync();

        }

        public async Task UpdateRecipientAsync(Recipient recipient)
        {
            var existingRecipient = await context.Recipients.FindAsync(recipient.Id);
            context.Entry(existingRecipient).CurrentValues.SetValues(recipient);
            await context.SaveChangesAsync();

        }
    }
}
