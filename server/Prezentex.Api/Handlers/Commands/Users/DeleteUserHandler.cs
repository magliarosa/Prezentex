using MediatR;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Commands.Users
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUsersRepository _usersRepository;

        public DeleteUserHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var isCorrectUser = request.RequestedUserId == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var existingUser = await _usersRepository.GetUserAsync(request.RequestedUserId);
            if (existingUser == null)
                throw new ResourceNotFoundException();

            await _usersRepository.DeleteUserAsync(request.RequestedUserId);

            return Unit.Value;
        }
    }
}
