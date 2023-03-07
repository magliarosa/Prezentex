using MediatR;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Commands.Users
{
    public class UpdateUserCommand : IRequest<User>
    {
        public UpdateUserCommand(string username, string email, Guid requestedUserId, Guid userId)
        {
            Username = username;
            Email = email;
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }

    }
}
