using MediatR;

namespace Prezentex.Api.Commands
{
    public class RemoveRecipientFromGiftCommand : IRequest<Unit>
    {
        public Guid RecipientId { get; set; }
        public Guid GiftId { get; set; }
        public Guid UserId { get; set; }

        public RemoveRecipientFromGiftCommand(Guid recipientId, Guid giftId, Guid userId)
        {
            RecipientId = recipientId;
            GiftId = giftId;
            UserId = userId;
        }
    }
}
