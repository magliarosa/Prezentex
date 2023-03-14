using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Gifts.Commands.DeleteGift
{
    public record DeleteGiftCommand : IRequest<Unit>
    {
        public Guid GiftId { get; set; }
        public Guid UserId { get; set; }

        public DeleteGiftCommand(Guid giftId, Guid userId)
        {
            GiftId = giftId;
            UserId = userId;
        }
    }

    public class DeleteGiftHandler : IRequestHandler<DeleteGiftCommand, Unit>
    {
        private readonly IGiftsRepository _giftsRepository;

        public DeleteGiftHandler(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }
        public async Task<Unit> Handle(DeleteGiftCommand request, CancellationToken cancellationToken)
        {
            var existingGift = await _giftsRepository.GetGiftAsync(request.GiftId);
            if (existingGift == null)
                throw new ResourceNotFoundException();

            var userOwnsGift = existingGift.UserId == request.UserId;
            if (!userOwnsGift)
                throw new ArgumentException("You do not own this gift");

            await _giftsRepository.DeleteGiftAsync(request.GiftId);

            return Unit.Value;
        }
    }
}
