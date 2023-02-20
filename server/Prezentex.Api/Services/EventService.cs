using Prezentex.Api.Services.Identity;
using Prezentex.Api.Services.Notifications;

namespace Prezentex.Api.Services
{
    public class EventService : IEventService
    {
        private IIdentityService _identityService;
        private INotificationsService _notificationsService;

        public EventService(INotificationsService notificationsService, IIdentityService identityService)
        {
            _notificationsService = notificationsService;
            _identityService = identityService;

        }

        public void SubscribeEvents()
        {
            _identityService.UserRegistered += (sender, user) => _notificationsService.GenerateUserCreatedNotification(user);
        }
    }
}
