using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Users.Queries.GetUser
{
    public record GetUserQuery : IRequest<User>
    {
        public GetUserQuery(Guid requestedUserId, Guid userId)
        {
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
    public class GetUserHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IUsersRepository _usersRepository;

        public GetUserHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {

            var isCorrectUser = request.RequestedUserId == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var user = await _usersRepository.GetUserAsync(request.RequestedUserId);

            if (user == null)
                throw new ResourceNotFoundException();

            return user;
        }
    }
}
