using MediatR;
using Prezentex.Api.Commands;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers
{
    public class UpdateGiftHandler : IRequestHandler<UpdateGiftCommand, Gift>
    {
        private readonly IGiftsRepository _giftsRepository;

        public UpdateGiftHandler(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }
        public async Task<Gift> Handle(UpdateGiftCommand request, CancellationToken cancellationToken)
        {
            var existingGift = await _giftsRepository.GetGiftAsync(request.GiftId);
            if (existingGift == null)
                throw new ResourceNotFoundException();

            var userOwnsGift = existingGift.UserId == request.UserId;
            if (!userOwnsGift)
                throw new ArgumentException("You do not own this gift.");

            var updatedGift = new Gift
            {
                Id = existingGift.Id,
                Description = request.Description,
                Name = request.Name,
                Price = request.Price,
                ProductUrl = request.ProductUrl,
                UpdatedDate = DateTimeOffset.UtcNow,
                Recipients = existingGift.Recipients,
                UserId = existingGift.UserId
            };

            await _giftsRepository.UpdateGiftAsync(updatedGift);

            return updatedGift;
        }
    }
}
