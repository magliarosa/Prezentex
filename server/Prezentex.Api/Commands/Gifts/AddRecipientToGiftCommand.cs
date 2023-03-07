using MediatR;

namespace Prezentex.Api.Commands.Gifts
{
    public class AddRecipientToGiftCommand : IRequest<Unit>
    {
        public AddRecipientToGiftCommand(Guid recipientId, Guid giftId, Guid userId)
        {
            RecipientId = recipientId;
            GiftId = giftId;
            UserId = userId;
        }

        public Guid RecipientId { get; set; }
        public Guid GiftId { get; set; }
        public Guid UserId { get; set; }
        
    }
}
