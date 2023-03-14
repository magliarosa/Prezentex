using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Recipients.Commands.DeleteRecipient
{
    public record DeleteRecipientCommand : IRequest<Unit>
    {
        public Guid RecipientId { get; set; }
        public Guid UserId { get; set; }

        public DeleteRecipientCommand(Guid recipientId, Guid userId)
        {
            RecipientId = recipientId;
            UserId = userId;
        }
    }
    public class DeleteRecipientHandler : IRequestHandler<DeleteRecipientCommand, Unit>
    {
        private readonly IRecipientsRepository _recipientsRepository;

        public DeleteRecipientHandler(IRecipientsRepository recipientsRepository)
        {
            _recipientsRepository = recipientsRepository;
        }
        public async Task<Unit> Handle(DeleteRecipientCommand request, CancellationToken cancellationToken)
        {
            var existingRecipient = await _recipientsRepository.GetRecipientAsync(request.RecipientId);
            if (existingRecipient == null)
                throw new ResourceNotFoundException();

            var userOwnsRecipient = existingRecipient.UserId == request.UserId;
            if (!userOwnsRecipient)
                throw new ArgumentException("You do not own this recipient");

            await _recipientsRepository.DeleteRecipientAsync(request.RecipientId);

            return Unit.Value;
        }
    }
}
