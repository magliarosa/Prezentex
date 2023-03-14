using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery : IRequest<IEnumerable<User>>
    {
        public GetAllUsersQuery() { }
    }
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
