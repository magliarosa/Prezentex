using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Domain.Entities;

namespace Prexentex.Application.Recipients.Commands.UpdateRecipient
{
    public record UpdateRecipientCommand : IRequest<Recipient>
    {
        public Guid RecipientId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }
        public Guid UserId { get; set; }

        public UpdateRecipientCommand(Guid recipientId, string name, string note, DateTimeOffset bithDay, DateTimeOffset nameDay, Guid userId)
        {
            Name = name;
            Note = note;
            BirthDay = bithDay;
            NameDay = nameDay;
            UserId = userId;
            RecipientId = recipientId;
        }
    }
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
