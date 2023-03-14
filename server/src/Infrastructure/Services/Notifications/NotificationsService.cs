using Prezentex.Api.Services.Notifications;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Infrastructure.Services.Notifications
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