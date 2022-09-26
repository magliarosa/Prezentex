using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories.Recipients;
using Swashbuckle.AspNetCore.Annotations;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipientsController : ControllerBase
    {
        private readonly IRecipientsRepository recipientsRepository;

        public RecipientsController(IRecipientsRepository recipientsRepository)
        {
            this.recipientsRepository = recipientsRepository;
        }

        [SwaggerOperation(Summary = "Get all recipients")]
        [HttpGet]
        public async Task<IEnumerable<RecipientDto>> GetRecipientsAsync()
        {
            var recipients = await recipientsRepository.GetRecipientsAsync();
            var recipientsDto = recipients.Select(recipient => recipient.AsDto());
            return recipientsDto;
        }

        [SwaggerOperation(Summary = "Get recipient by ID")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipientDto>> GetRecipientAsync(Guid id)
        {
            var recipient = await recipientsRepository.GetRecipientAsync(id);

            if (recipient == null)
                return NotFound();

            return recipient.AsDto();
        }

        [SwaggerOperation(Summary = "Create recipient")]
        [HttpPost]
        public async Task<ActionResult<RecipientDto>> CreateRecipientAsync(CreateRecipientDto recipientDto)
        {
            var newRecipient = new Recipient()
            {
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Name = recipientDto.Name,
                BirthDay = recipientDto.BirthDay,
                NameDay = recipientDto.NameDay,
                Note = recipientDto.Note
            };

            await recipientsRepository.CreateRecipientAsync(newRecipient);

            return CreatedAtAction(nameof(GetRecipientAsync), new { Id = newRecipient.Id }, newRecipient.AsDto());
        }

        [SwaggerOperation(Summary = "Update recipient")]
        [HttpPut("{id}")]
        public async Task<ActionResult<GiftDto>> UpdateRecipientAsync(Guid id, UpdateRecipientDto recipientDto)
        {
            var existingRecipient = await recipientsRepository.GetRecipientAsync(id);

            if (existingRecipient == null)
                return NotFound();

            var updatedRecipient = new Recipient
            {
                Id = existingRecipient.Id,
                Name = recipientDto.Name,
                UpdatedDate = DateTimeOffset.UtcNow,
                BirthDay = recipientDto.BirthDay,
                NameDay = recipientDto.NameDay,
                CreatedDate = existingRecipient.CreatedDate,
                Gifts = existingRecipient.Gifts,
                Note = recipientDto.Note
            };

            await recipientsRepository.UpdateRecipientAsync(updatedRecipient);

            return Ok(updatedRecipient.AsDto());
        }

        [SwaggerOperation(Summary = "Delete recipient")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipientAsync(Guid id)
        {
            var existingRecipient = await recipientsRepository.GetRecipientAsync(id);
            if (existingRecipient == null)
                return NotFound();

            await recipientsRepository.DeleteRecipientAsync(id);

            return NoContent();
        }

        [SwaggerOperation(Summary = "Add gift to recipient by Ids")]
        [HttpPost("{recipientId}")]
        public async Task<ActionResult> AddGiftToRecipientAsync(Guid recipientId, [FromBody] Guid giftId)
        {
            var recipient = await recipientsRepository.GetRecipientAsync(recipientId);
            if (recipient == null)
                return NotFound();

            await recipientsRepository.AddGiftToRecipientAsync(recipientId, giftId);

            return NoContent();
        }

        
        [SwaggerOperation(Summary = "Remove gift from recipient by Ids")]
        [HttpDelete("{recipientId}/gifts")]
        public async Task<ActionResult> RemoveGiftFromRecipientAsync(Guid recipientId, [FromBody] Guid giftId)
        {
            var recipient = await recipientsRepository.GetRecipientAsync(recipientId);
            if (recipient == null)
                return NotFound();

            await recipientsRepository.RemoveGiftFromRecipientAsync(recipientId, giftId);

            return NoContent();
        }
    }
}
