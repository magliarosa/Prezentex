using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Users.Commands.UpdateUser
{
    public record UpdateUserCommand : IRequest<User>
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
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
    {
        private readonly IUsersRepository _usersRepository;

        public UpdateUserHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {

            var isCorrectUser = request.RequestedUserId == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var existingUser = await _usersRepository.GetUserAsync(request.RequestedUserId);

            if (existingUser == null)
                throw new ResourceNotFoundException();

            var updatedUser = new User
            {
                Id = existingUser.Id,
                UserName = request.Username,
                Gifts = existingUser.Gifts,
                Recipients = existingUser.Recipients,
                Email = request.Email
            };

            await _usersRepository.UpdateUserAsync(updatedUser);

            return updatedUser;

        }
    }
}
