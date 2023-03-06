using MediatR;

namespace Prezentex.Api.Commands.Gifts
{
    public class DeleteGiftCommand : IRequest<Unit>
    {
        public Guid GiftId { get; set; }
        public Guid UserId { get; set; }

        public DeleteGiftCommand(Guid giftId, Guid userId)
        {
            GiftId = giftId;
            UserId = userId;
        }
    }
}
