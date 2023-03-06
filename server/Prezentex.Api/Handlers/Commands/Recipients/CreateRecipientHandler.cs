using MediatR;
using Prezentex.Api.Commands.Recipients;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers.Commands.Recipients
{
    public class CreateRecipientHandler : IRequestHandler<CreateRecipientCommand, Recipient>
    {
        private readonly IRecipientsRepository _recipientsRepository;

        public CreateRecipientHandler(IRecipientsRepository recipientsRepository)
        {
            _recipientsRepository = recipientsRepository;
        }
        
        public async Task<Recipient> Handle(CreateRecipientCommand request, CancellationToken cancellationToken)
        {

            var newRecipient = new Recipient()
            {
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Name = request.Name,
                BirthDay = request.BirthDay,
                NameDay = request.NameDay,
                Note = request.Note,
                UserId = request.UserId
            };

            await _recipientsRepository.CreateRecipientAsync(newRecipient);

            return newRecipient;
        }
    }
}
