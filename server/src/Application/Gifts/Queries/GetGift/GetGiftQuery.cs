using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Gifts.Queries.GetGift
{
    public record GetGiftQuery : IRequest<Gift>
    {
        public Guid GiftId { get; }
        public Guid UserId { get; }

        public GetGiftQuery(Guid giftId, Guid userId)
        {
            GiftId = giftId;
            UserId = userId;
        }
    }

    public class GetGiftHandler : IRequestHandler<GetGiftQuery, Gift>
    {
        private readonly IGiftsRepository _giftsRepository;

        public GetGiftHandler(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }

        public async Task<Gift> Handle(GetGiftQuery request, CancellationToken cancellationToken)
        {

            var gift = await _giftsRepository.GetGiftAsync(request.GiftId);

            if (gift == null)
                throw new ResourceNotFoundException();

            var userOwnsGift = gift.UserId == request.UserId;
            if (!userOwnsGift)
                throw new ArgumentException("You do not own this gift");

            return gift;
        }
    }
}
