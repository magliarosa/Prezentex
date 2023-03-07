using MediatR;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Commands.Users
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IUsersRepository _usersRepository;

        public CreateUserHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var newUser = new User
            {
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email
            };

            await _usersRepository.CreateUserAsync(newUser);

            return newUser;
        }
    }
}
