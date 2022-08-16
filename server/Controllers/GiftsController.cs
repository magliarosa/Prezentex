using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Dtos;
using Prezentex.Entities;
using Prezentex.Repositories;

namespace Prezentex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftsRepository _giftsRepository;

        public GiftsController(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }

        [HttpGet]
        public IEnumerable<GiftDto> GetGifts()
        {
            var gifts = _giftsRepository.GetGifts();
            var giftsDto = gifts.Select(gift => gift.AsDto());
            return giftsDto;
        }

        [HttpGet("id")]
        public ActionResult<GiftDto> GetGift(Guid id)
        {
            var gift = _giftsRepository.GetGift(id);

            if (gift == null)
                return NotFound();

            return gift.AsDto();
        }

        [HttpPost]
        public ActionResult<GiftDto> CreateGift(CreateGiftDto giftDto)
        {
            var newGift = new Gift()
            {
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now,
                Description = giftDto.Description,
                Id = Guid.NewGuid(),
                Name = giftDto.Name,
                Price = giftDto.Price,
                ProductUrl = giftDto.ProductUrl
            };

            _giftsRepository.CreateGift(newGift);

            return CreatedAtAction(nameof(CreateGift), newGift.AsDto());
        }

        [HttpPut("{id}")]
        public ActionResult<GiftDto> UpdateGift(Guid id, UpdateGiftDto giftDto)
        {
            var existingGift = _giftsRepository.GetGift(id);

            if (existingGift == null)
                return NotFound();

            var updatedGift = existingGift with
            {
                Description = giftDto.Description,
                Name = giftDto.Name,
                Price = giftDto.Price,
                ProductUrl = giftDto.ProductUrl,
                UpdatedDate = DateTimeOffset.Now
            };

            _giftsRepository.UpdateGift(updatedGift);

            return Ok(updatedGift.AsDto());
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteGift(Guid id)
        {
            _giftsRepository.DeleteGift(id);

            return NoContent();
        }

    }
}
