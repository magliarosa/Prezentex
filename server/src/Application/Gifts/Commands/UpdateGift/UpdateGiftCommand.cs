using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Application.Gifts.Commands.UpdateGift
{
    public record UpdateGiftCommand : IRequest<Gift>
    {
        [Required]
        public Guid GiftId { get; set; }
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductUrl { get; set; }
        public Guid UserId { get; set; }

        public UpdateGiftCommand(Guid giftId, string name, string description, decimal price, string productUrl, Guid userId)
        {
            Name = name;
            Description = description;
            Price = price;
            ProductUrl = productUrl;
            UserId = userId;
            GiftId = giftId;
        }
    }

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
