using MediatR;
using Prezentex.Api.Commands.Recipients;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers.Commands.Recipients
{
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
