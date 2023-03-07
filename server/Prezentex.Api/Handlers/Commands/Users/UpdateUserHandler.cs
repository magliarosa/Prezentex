using MediatR;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Commands.Users
{
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
                Username = request.Username,
                UpdatedDate = DateTimeOffset.UtcNow,
                CreatedDate = existingUser.CreatedDate,
                Gifts = existingUser.Gifts,
                Recipients = existingUser.Recipients,
                Email = request.Email
            };

            await _usersRepository.UpdateUserAsync(updatedUser);

            return updatedUser;

        }
    }
}
