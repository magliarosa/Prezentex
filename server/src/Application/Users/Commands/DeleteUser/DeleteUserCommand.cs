using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Users.Commands.DeleteUser
{
    public record DeleteUserCommand : IRequest<Unit>
    {
        public DeleteUserCommand(Guid requestedUserId, Guid userId)
        {
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
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
