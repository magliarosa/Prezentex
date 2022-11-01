using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories;
using Prezentex.Api.Repositories.Recipients;
using Swashbuckle.AspNetCore.Annotations;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftsRepository giftsRepository;
        private readonly IRecipientsRepository recipientsRepository;

        public GiftsController(IGiftsRepository giftsRepository, IRecipientsRepository recipientsRepository)
        {
            this.giftsRepository = giftsRepository;
            this.recipientsRepository = recipientsRepository;
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
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Description = giftDto.Description,
                Id = Guid.NewGuid(),
                Name = giftDto.Name,
                Price = giftDto.Price,
                ProductUrl = giftDto.ProductUrl
            };

            await giftsRepository.CreateGiftAsync(newGift);

            return CreatedAtAction(nameof(GetGiftAsync), new { Id = newGift.Id }, newGift.AsDto());
        }

        [SwaggerOperation(Summary = "Update gift")]
        [HttpPut("{id}")]
        public async Task<ActionResult<GiftDto>> UpdateGiftAsync(Guid id, UpdateGiftDto giftDto)
        {
            var existingGift = await giftsRepository.GetGiftAsync(id);

            if (existingGift == null)
                return NotFound();

            var updatedGift = new Gift
            {
                Id = existingGift.Id,
                Description = giftDto.Description,
                Name = giftDto.Name,
                Price = giftDto.Price,
                ProductUrl = giftDto.ProductUrl,
                UpdatedDate = DateTimeOffset.UtcNow,
                Recipients = existingGift.Recipients
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

        [SwaggerOperation(Summary = "Add recipient to gift by Id")]
        [HttpPost("{giftId}/recipients")]
        public async Task<ActionResult> AddRecipientToGiftAsync(Guid giftId, AddRecipientToGiftDto addRecipientToGiftDto)
        {
            var gift = await giftsRepository.GetGiftAsync(giftId);
            var recipientId = addRecipientToGiftDto.RecipientId;
            var recipient = await recipientsRepository.GetRecipientAsync(recipientId);

            if (gift == null || recipient == null)
                return NotFound();

            await giftsRepository.AddRecipientToGiftAsync(giftId, recipientId);

            return NoContent();
        }


        [SwaggerOperation(Summary = "Remove recipient from gift by Id")]
        [HttpDelete("{giftId}/recipients")]
        public async Task<ActionResult> RemoveRecipientFromGiftAsync(Guid giftId, RemoveRecipientFromGiftDto removeRecipientFromGiftDto)
        {
            var gift = await giftsRepository.GetGiftAsync(giftId);
            var recipientId = removeRecipientFromGiftDto.RecipientId;
            var recipient = await recipientsRepository.GetRecipientAsync(recipientId);

            if (gift == null || recipient == null || !gift.Recipients.Any(recipient => recipient.Id == recipientId))
                return NotFound();

            await giftsRepository.RemoveRecipientFromGiftAsync(giftId, recipientId);

            return NoContent();
        }
    }
}
