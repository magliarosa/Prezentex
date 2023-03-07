using MediatR;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Queries.Users
{
    public class GetUserQuery : IRequest<User>
    {
        public GetUserQuery(Guid requestedUserId, Guid userId)
        {
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
}
