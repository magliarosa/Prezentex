using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Commands;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories;
using Prezentex.Api.Controllers;

namespace Prezentex.Api.Handlers
{
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
