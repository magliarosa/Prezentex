using Prezentex.Dtos;
using Prezentex.Entities;

namespace Prezentex
{
    public static class Extensions
    {
        public static GiftDto AsDto(this Gift gift)
        {
            return new GiftDto
            {
                CreatedDate = gift.CreatedDate,
                Description = gift.Description,
                Id = gift.Id,
                Name = gift.Name,
                Price = gift.Price,
                ProductUrl = gift.ProductUrl
            };
        }
    }
}
