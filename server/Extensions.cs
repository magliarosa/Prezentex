using Prezentex.Dtos;
using Prezentex.Entities;

namespace Prezentex
{
    public static class Extensions
    {
        public static GiftDto AsDto(this Gift gift)
        {
            return new GiftDto(gift.Id, gift.CreatedDate, gift.Name, gift.Description, gift.Price, gift.ProductUrl);
        }
    }
}
