using MediatR;

namespace Prezentex.Api.Commands.Users
{
    public class RemoveGiftFromUserCommand : IRequest<Unit>
    {
        public RemoveGiftFromUserCommand(Guid giftId, Guid requestedUserId, Guid userId)
        {
            GiftId = giftId;
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid GiftId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }

    }
}
