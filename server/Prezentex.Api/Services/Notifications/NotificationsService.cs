using Prezentex.Api.Entities;
using Prezentex.Api.Repositories.Notifications;
using Prezentex.Api.Services.Identity;

namespace Prezentex.Api.Services.Notifications
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _notificationsRepository;

        public NotificationsService(INotificationsRepository notificationsRepository)
        {
            _notificationsRepository = notificationsRepository;
        }

        public void GenerateUserCreatedNotification(User user)
        {
            var notf = new Notification
            {
                Content = "Your account has been created!",
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                Receiver = user.Email
            };
            _notificationsRepository.CreateNotificationAsync(notf);
        }
    }
}