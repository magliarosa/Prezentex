using MediatR;
using Prezentex.Api.Commands.Recipients;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers.Commands.Recipients
{
    public class UpdateRecipientHandler : IRequestHandler<UpdateRecipientCommand, Recipient>
    {
        private readonly IRecipientsRepository _recipientsRepository;

        public UpdateRecipientHandler(IRecipientsRepository recipientsRepository)
        {
            _recipientsRepository = recipientsRepository;
        }

        public async Task<Recipient> Handle(UpdateRecipientCommand request, CancellationToken cancellationToken)
        {
            var existingRecipient = await _recipientsRepository.GetRecipientAsync(request.RecipientId);

            if (existingRecipient == null)
                throw new ResourceNotFoundException();

            var userOwnsRecipient = existingRecipient.UserId == request.UserId;
            if (!userOwnsRecipient)
                throw new ArgumentException("You do not own this recipient");

            var updatedRecipient = new Recipient
            {
                Id = existingRecipient.Id,
                Name = request.Name,
                UpdatedDate = DateTimeOffset.UtcNow,
                BirthDay = request.BirthDay,
                NameDay = request.NameDay,
                CreatedDate = existingRecipient.CreatedDate,
                Note = request.Note
            };

            await _recipientsRepository.UpdateRecipientAsync(updatedRecipient);

            return updatedRecipient;
        }
    }
}
