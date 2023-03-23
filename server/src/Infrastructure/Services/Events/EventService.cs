using Prezentex.Api.Services.Notifications;
using Prezentex.Application.Common.Interfaces.Events;
using Prezentex.Application.Common.Interfaces.Identity;

namespace Prezentex.Infrastructure.Services.Events
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
