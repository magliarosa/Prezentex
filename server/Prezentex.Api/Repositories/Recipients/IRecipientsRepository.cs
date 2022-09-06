using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories.Recipients
{
    public interface IRecipientsRepository
    {
        Task<Recipient> GetRecipientAsync(Guid id);
        Task<IEnumerable<Recipient>> GetRecipientsAsync();
        Task CreateRecipientAsync(Recipient recipient);
        Task UpdateRecipientAsync(Recipient recipient);
        Task DeleteRecipientAsync(Guid id);
        Task AddGiftToRecipientAsync(Guid recipientId, Guid giftId);
        Task RemoveGiftFromRecipientAsync(Guid recipientId, Guid giftId);
    }
}
