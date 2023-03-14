using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Users.Commands.AddRecipientToUser
{
    public record AddRecipientToUserCommand : IRequest<Unit>
    {
        public AddRecipientToUserCommand(Guid recipientId, Guid requestedUserId, Guid userId)
        {
            RecipientId = recipientId;
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid RecipientId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
    public class AddRecipientToUserHandler : IRequestHandler<AddRecipientToUserCommand, Unit>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IRecipientsRepository _recipientsRepository;

        public AddRecipientToUserHandler(IUsersRepository usersRepository, IRecipientsRepository recipientsRepository)
        {
            _usersRepository = usersRepository;
            _recipientsRepository = recipientsRepository;
        }
        public async Task<Unit> Handle(AddRecipientToUserCommand request, CancellationToken cancellationToken)
        {
            var isCorrectUser = request.RequestedUserId == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var user = await _usersRepository.GetUserAsync(request.RequestedUserId);
            var recipient = await _recipientsRepository.GetRecipientAsync(request.RecipientId);

            if (user == null || recipient == null)
                throw new ResourceNotFoundException();


            var userOwnsRecipient = recipient.UserId == user.Id;
            if (!userOwnsRecipient)
                throw new ArgumentException("User does not own this recipient");

            await _usersRepository.AddRecipientToUserAsync(request.RequestedUserId, request.RecipientId);

            return Unit.Value;
        }
    }
}
