using Prezentex.Domain.Entities;

namespace Prezentex.Application.Common.Interfaces.Repositories
{
    public interface IRecipientsRepository
    {
        Task<Recipient> GetRecipientAsync(Guid id);
        Task<IEnumerable<Recipient>> GetRecipientsAsync();
        Task CreateRecipientAsync(Recipient recipient);
        Task UpdateRecipientAsync(Recipient recipient);
        Task DeleteRecipientAsync(Guid id);
        Task<bool> UserOwnsRecipientAsync(Guid recipientId, Guid userId);
    }
}
