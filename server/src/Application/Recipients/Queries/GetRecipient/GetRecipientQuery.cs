using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Recipients.Queries.GetRecipient
{
    public record GetRecipientQuery : IRequest<Recipient>
    {
        public Guid RecipientId { get; set; }
        public Guid UserId { get; set; }

        public GetRecipientQuery(Guid recipientId, Guid userId)
        {
            RecipientId = recipientId;
            UserId = userId;
        }
    }
    public class GetRecipientHandler : IRequestHandler<GetRecipientQuery, Recipient>
    {
        private readonly IRecipientsRepository _recipientsRepository;

        public GetRecipientHandler(IRecipientsRepository recipientsRepository)
        {
            _recipientsRepository = recipientsRepository;
        }

        public async Task<Recipient> Handle(GetRecipientQuery request, CancellationToken cancellationToken)
        {

            var recipient = await _recipientsRepository.GetRecipientAsync(request.RecipientId);

            if (recipient == null)
                throw new ResourceNotFoundException();

            var userOwnsRecipient = recipient.UserId == request.UserId;
            if (!userOwnsRecipient)
                throw new ArgumentException("You do not own this recipient");

            return recipient;
        }
    }
}
