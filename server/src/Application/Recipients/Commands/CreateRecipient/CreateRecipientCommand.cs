using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Recipients.Commands.CreateRecipient
{
    public record CreateRecipientCommand : IRequest<Recipient>
    {
        public string Name { get; set; }
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }
        public string Note { get; set; }
        public Guid UserId { get; set; }

        public CreateRecipientCommand(string name, DateTimeOffset birthDay, DateTimeOffset nameDay, string note, Guid userId)
        {
            Name = name;
            BirthDay = birthDay;
            NameDay = nameDay;
            Note = note;
            UserId = userId;
        }
    }

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
