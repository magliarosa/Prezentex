using MediatR;
using Prezentex.Api.Commands.Gifts;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers.Commands.Users
{
    public class RemoveRecipientFromUserHandler : IRequestHandler<RemoveRecipientFromUserCommand, Unit>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IRecipientsRepository _recipientsRepository;

        public RemoveRecipientFromUserHandler(IUsersRepository usersRepository, IRecipientsRepository recipientsRepository)
        {
            _usersRepository = usersRepository;
            _recipientsRepository = recipientsRepository;
        }
        public async Task<Unit> Handle(RemoveRecipientFromUserCommand request, CancellationToken cancellationToken)
        {

            var isCorrectUser = request.RequestedUserId == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var user = await _usersRepository.GetUserAsync(request.RequestedUserId);
            var recipient = await _recipientsRepository.GetRecipientAsync(request.RecipientId);

            if (user == null || recipient == null || !user.Recipients.Any(recipient => recipient.Id == request.RecipientId))
                throw new ResourceNotFoundException();

            var userOwnsRecipient = recipient.UserId == user.Id;
            if (!userOwnsRecipient)
                throw new ArgumentException("User does not own this recipient");

            await _usersRepository.RemoveRecipientFromUserAsync(request.RequestedUserId, request.RecipientId);

            return Unit.Value;
        }
    }
}
