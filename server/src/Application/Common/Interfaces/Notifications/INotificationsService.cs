using Prezentex.Domain.Entities;

namespace Prezentex.Api.Services.Notifications
{
    public interface INotificationsService
    {
        public void GenerateUserCreatedNotification(User user);
    }
}
