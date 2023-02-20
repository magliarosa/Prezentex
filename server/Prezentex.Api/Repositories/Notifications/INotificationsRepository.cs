using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories.Notifications
{
    public interface INotificationsRepository
    {
        Task<Notification> GetNotificationAsync(Guid id);
        Task<IEnumerable<Notification>> GetNotificationsAsync();
        Task CreateNotificationAsync(Notification notification);
    }
}
