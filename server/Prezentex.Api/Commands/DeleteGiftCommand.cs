using MediatR;

namespace Prezentex.Api.Commands
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
