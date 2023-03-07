using MediatR;

namespace Prezentex.Api.Commands.Users
{
    public class AddRecipientToUserCommand : IRequest<Unit>
    {
        public AddRecipientToUserCommand(Guid recipientId, Guid requestedUserId, Guid userId)
        {
            RecipientId = recipientId;
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid RecipientId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
}
