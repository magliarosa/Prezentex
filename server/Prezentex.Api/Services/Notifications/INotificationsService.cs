using Prezentex.Api.Entities;

namespace Prezentex.Api.Services.Notifications
{
    public interface INotificationsService
    {
        public void GenerateUserCreatedNotification(User user);
    }
}
