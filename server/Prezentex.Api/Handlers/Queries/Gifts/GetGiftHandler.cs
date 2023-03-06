using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Queries.Gifts;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Queries.Gifts
{
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
