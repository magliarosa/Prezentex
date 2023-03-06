using MediatR;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Queries.Recipients
{
    public class GetRecipientQuery : IRequest<Recipient>
    {
        public Guid RecipientId { get; set; }
        public Guid UserId { get; set; }

        public GetRecipientQuery(Guid recipientId, Guid userId)
        {
            RecipientId = recipientId;
            UserId = userId;
        }
    }
}
