using Prezentex.Domain.Entities;

namespace Prezentex.Application.Common.Interfaces.Repositories
{
    public interface INotificationsRepository
    {
        Task<Notification> GetNotificationAsync(Guid id);
        Task<IEnumerable<Notification>> GetNotificationsAsync();
        Task CreateNotificationAsync(Notification notification);
    }
}
