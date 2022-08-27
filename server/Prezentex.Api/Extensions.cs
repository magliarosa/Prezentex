using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;

namespace Prezentex.Api
{
    public static class Extensions
    {
        public static GiftDto AsDto(this Gift gift)
        {
            return new GiftDto(gift.Id, gift.CreatedDate, gift.Name, gift.Description, gift.Price, gift.ProductUrl);
        }
    }
}
