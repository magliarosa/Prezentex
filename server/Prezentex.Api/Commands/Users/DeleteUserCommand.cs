using MediatR;

namespace Prezentex.Api.Commands.Users
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public DeleteUserCommand(Guid requestedUserId, Guid userId)
        {
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
}
