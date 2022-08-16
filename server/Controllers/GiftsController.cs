using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Dtos;
using Prezentex.Entities;
using Prezentex.Repositories;
using Swashbuckle.AspNetCore.Annotations;

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

        [SwaggerOperation(Summary = "Get all gifts")]
        [HttpGet]
        public IEnumerable<GiftDto> GetGifts()
        {
            var gifts = _giftsRepository.GetGifts();
            var giftsDto = gifts.Select(gift => gift.AsDto());
            return giftsDto;
        }

        [SwaggerOperation(Summary = "Get gift by ID")]
        [HttpGet("id")]
        public ActionResult<GiftDto> GetGift(Guid id)
        {
            var gift = _giftsRepository.GetGift(id);

            if (gift == null)
                return NotFound();

            return gift.AsDto();
        }

        [SwaggerOperation(Summary = "Create gift")]
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

        [SwaggerOperation(Summary = "Update gift")]
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

        [SwaggerOperation(Summary = "Delete gift")]
        [HttpDelete("{id}")]
        public ActionResult DeleteGift(Guid id)
        {
            _giftsRepository.DeleteGift(id);

            return NoContent();
        }

    }
}
