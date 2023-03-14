using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prexentex.Application.Recipients.Commands.UpdateRecipient;
using Prezentex.Api.Dtos;
using Prezentex.Application.Recipients.Commands.CreateRecipient;
using Prezentex.Application.Recipients.Commands.DeleteRecipient;
using Prezentex.Application.Recipients.Queries.GetAllRecipients;
using Prezentex.Application.Recipients.Queries.GetRecipient;
using Swashbuckle.AspNetCore.Annotations;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class RecipientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecipientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(Summary = "Get all recipients")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipientDto>>> GetRecipientsAsync()
        {
            var query = new GetAllRecipientsQuery();
            var result = await _mediator.Send(query);
            var giftsDto = result.Select(x => x.AsDto());
            return Ok(giftsDto);
        }

        [SwaggerOperation(Summary = "Get recipient by ID")]
        [HttpGet("{recipientId}")]
        public async Task<ActionResult<RecipientDto>> GetRecipientAsync(Guid recipientId)
        {
            var query = new GetRecipientQuery(recipientId, HttpContext.GetUserId());
            var result = await _mediator.Send(query);
            var giftDto = result.AsDto();
            return giftDto;
        }

        [SwaggerOperation(Summary = "Create recipient")]
        [HttpPost]
        public async Task<ActionResult<RecipientDto>> CreateRecipientAsync(CreateRecipientDto recipientDto)
        {
            var command = new CreateRecipientCommand(recipientDto.Name, recipientDto.BirthDay, recipientDto.NameDay, recipientDto.Note, HttpContext.GetUserId());
            var result = await _mediator.Send(command);
            var newRecipientDto = result.AsDto();
            return CreatedAtAction(nameof(GetRecipientAsync), new { Id = newRecipientDto.Id }, newRecipientDto);
        }

        [SwaggerOperation(Summary = "Update recipient")]
        [HttpPut("{recipitentId}")]
        public async Task<ActionResult<RecipientDto>> UpdateRecipientAsync(Guid recipitentId, UpdateRecipientDto recipientDto)
        {
            var command = new UpdateRecipientCommand(
                recipitentId,
                recipientDto.Note,
                recipientDto.Name,
                recipientDto.BirthDay,
                recipientDto.NameDay,
                HttpContext.GetUserId());
            var result = await _mediator.Send(command);
            return result.AsDto();
        }

        [SwaggerOperation(Summary = "Delete recipient")]
        [HttpDelete("{recipientId}")]
        public async Task<ActionResult> DeleteRecipientAsync(Guid recipientId)
        {
            var command = new DeleteRecipientCommand(recipientId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
