using MediatR;
using Prezentex.Api.Entities;
using Prezentex.Api.Queries.Users;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Queries.Users
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
    {
        private readonly IUsersRepository _usersRepository;

        public GetAllUsersHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _usersRepository.GetUsersAsync();
            return users;
        }
    }
}
