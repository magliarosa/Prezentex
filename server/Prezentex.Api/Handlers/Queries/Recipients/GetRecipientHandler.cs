using MediatR;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Queries.Recipients;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers.Queries.Recipients
{
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
