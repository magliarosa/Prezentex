using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Application.Gifts.Commands.CreateGift
{
    public record CreateGiftCommand : IRequest<Gift>
    {
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductUrl { get; set; }
        public Guid UserId { get; set; }

        public CreateGiftCommand(string name, string description, decimal price, string productUrl, Guid userId)
        {
            Name = name;
            Description = description;
            Price = price;
            ProductUrl = productUrl;
            UserId = userId;
        }
    }
    public class CreateGiftHandler : IRequestHandler<CreateGiftCommand, Gift>
    {
        private readonly IGiftsRepository _giftsRepository;

        public CreateGiftHandler(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }
        public async Task<Gift> Handle(CreateGiftCommand request, CancellationToken cancellationToken)
        {
            var newGift = new Gift()
            {
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Description = request.Description,
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                ProductUrl = request.ProductUrl,
                UserId = request.UserId
            };

            await _giftsRepository.CreateGiftAsync(newGift);

            return newGift;
        }
    }
}
