using MediatR;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Queries.Users;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Queries.Users
{
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
