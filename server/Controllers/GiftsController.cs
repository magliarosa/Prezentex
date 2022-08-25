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
        private readonly IGiftsRepository giftsRepository;

        public GiftsController(IGiftsRepository giftsRepository)
        {
            this.giftsRepository = giftsRepository;
        }

        [SwaggerOperation(Summary = "Get all gifts")]
        [HttpGet]
        public async Task<IEnumerable<GiftDto>> GetGiftsAsync()
        {
            var gifts = await giftsRepository.GetGiftsAsync();
            var giftsDto = gifts.Select(gift => gift.AsDto());
            return giftsDto;
        }

        [SwaggerOperation(Summary = "Get gift by ID")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GiftDto>> GetGiftAsync(Guid id)
        {
            var gift = await giftsRepository.GetGiftAsync(id);

            if (gift == null)
                return NotFound();

            return gift.AsDto();
        }

        [SwaggerOperation(Summary = "Create gift")]
        [HttpPost]
        public async Task<ActionResult<GiftDto>> CreateGiftAsync(CreateGiftDto giftDto)
        {
            var newGift = new Gift()
            {
                CreatedDate = DateTimeOffset.Now.ToUniversalTime(),
                UpdatedDate = DateTimeOffset.Now.ToUniversalTime(),
                Description = giftDto.Description,
                Id = Guid.NewGuid(),
                Name = giftDto.Name,
                Price = giftDto.Price,
                ProductUrl = giftDto.ProductUrl
            };

            await giftsRepository.CreateGiftAsync(newGift);

            return CreatedAtAction(nameof(GetGiftAsync), new {Id = newGift.Id}, newGift.AsDto());
        }

        [SwaggerOperation(Summary = "Update gift")]
        [HttpPut("{id}")]
        public async Task<ActionResult<GiftDto>> UpdateGiftAsync(Guid id, UpdateGiftDto giftDto)
        {
            var existingGift = await giftsRepository.GetGiftAsync(id);

            if (existingGift == null)
                return NotFound();

            var updatedGift = existingGift with
            {
                Description = giftDto.Description,
                Name = giftDto.Name,
                Price = giftDto.Price,
                ProductUrl = giftDto.ProductUrl,
                UpdatedDate = DateTimeOffset.Now.ToUniversalTime()
            };

            await giftsRepository.UpdateGiftAsync(updatedGift);

            return Ok(updatedGift.AsDto());
        }

        [SwaggerOperation(Summary = "Delete gift")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGiftAsync(Guid id)
        {
            var existingGift = await giftsRepository.GetGiftAsync(id);
            if (existingGift == null)
                return NotFound();

            await giftsRepository.DeleteGiftAsync(id);

            return NoContent();
        }

    }
}
