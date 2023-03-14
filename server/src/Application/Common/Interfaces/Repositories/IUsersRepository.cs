using Prezentex.Domain.Entities;

namespace Prezentex.Application.Common.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(Guid id);
        Task<IEnumerable<User>> GetUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task AddGiftToUserAsync(Guid userId, Guid giftId);
        Task RemoveGiftFromUserAsync(Guid userId, Guid giftId);
        Task AddRecipientToUserAsync(Guid userId, Guid recipientId);
        Task RemoveRecipientFromUserAsync(Guid userId, Guid recipientId);

    }
}
