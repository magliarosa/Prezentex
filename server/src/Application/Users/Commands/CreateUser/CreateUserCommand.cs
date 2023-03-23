using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Users.Commands.CreateUser
{
    public record CreateUserCommand : IRequest<User>
    {
        public CreateUserCommand(string username, string email)
        {
            Username = username;
            Email = email;
        }

        public string Username { get; set; }
        public string Email { get; set; }

    }
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
                Id = Guid.NewGuid(),
                UserName = request.Username,
                Email = request.Email
            };

            await _usersRepository.CreateUserAsync(newUser);

            return newUser;
        }
    }
}
