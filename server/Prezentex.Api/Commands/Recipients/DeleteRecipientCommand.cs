using MediatR;

namespace Prezentex.Api.Commands.Recipients
{
    public class DeleteRecipientCommand : IRequest<Unit>
    {
        public Guid RecipientId { get; set; }
        public Guid UserId { get; set; }

        public DeleteRecipientCommand(Guid recipientId, Guid userId)
        {
            RecipientId = recipientId;
            UserId = userId;
        }
    }
}
